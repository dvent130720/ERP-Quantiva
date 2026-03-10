#!/usr/bin/env bash
set -u

pass_count=0
fail_count=0

print_result() {
  local name="$1"
  local status="$2"
  local detail="$3"

  if [[ "$status" == "PASS" ]]; then
    echo "PASS | ${name} | ${detail}"
    pass_count=$((pass_count + 1))
  else
    echo "FAIL | ${name} | ${detail}"
    fail_count=$((fail_count + 1))
  fi
}

# 1) Ubuntu version
if [[ -f /etc/os-release ]]; then
  ubuntu_version="$(awk -F= '/^VERSION_ID=/{gsub(/"/,"",$2); print $2}' /etc/os-release)"
  ubuntu_name="$(awk -F= '/^NAME=/{gsub(/"/,"",$2); print $2}' /etc/os-release)"
  if [[ "$ubuntu_name" == "Ubuntu" && -n "$ubuntu_version" ]]; then
    print_result "Ubuntu version" "PASS" "${ubuntu_name} ${ubuntu_version}"
  else
    print_result "Ubuntu version" "FAIL" "Sistema detectado: ${ubuntu_name:-desconocido}"
  fi
else
  print_result "Ubuntu version" "FAIL" "/etc/os-release no encontrado"
fi

# 2) Docker installed
if command -v docker >/dev/null 2>&1; then
  docker_version="$(docker --version 2>/dev/null || true)"
  print_result "Docker installed" "PASS" "${docker_version:-docker encontrado}"
else
  print_result "Docker installed" "FAIL" "docker no está instalado"
fi

# 3) Docker daemon running
if command -v docker >/dev/null 2>&1; then
  if docker info >/dev/null 2>&1; then
    print_result "Docker daemon running" "PASS" "daemon activo"
  else
    print_result "Docker daemon running" "FAIL" "daemon inactivo o sin permisos"
  fi
else
  print_result "Docker daemon running" "FAIL" "docker no está instalado"
fi

# 4) Docker compose installed
if docker compose version >/dev/null 2>&1; then
  compose_version="$(docker compose version 2>/dev/null | head -n1)"
  print_result "Docker compose installed" "PASS" "${compose_version}"
elif command -v docker-compose >/dev/null 2>&1; then
  compose_version="$(docker-compose --version 2>/dev/null | head -n1)"
  print_result "Docker compose installed" "PASS" "${compose_version}"
else
  print_result "Docker compose installed" "FAIL" "docker compose no disponible"
fi

# 5) Git installed
if command -v git >/dev/null 2>&1; then
  git_version="$(git --version 2>/dev/null)"
  print_result "Git installed" "PASS" "${git_version}"
else
  print_result "Git installed" "FAIL" "git no está instalado"
fi

# 6) SSH access working
# Verifica que haya llave y que github.com responda handshake (códigos 1/255 pueden variar por permisos de cuenta)
if command -v ssh >/dev/null 2>&1; then
  has_key="false"
  for key in "$HOME/.ssh/id_ed25519" "$HOME/.ssh/id_rsa"; do
    if [[ -f "$key" ]]; then
      has_key="true"
      break
    fi
  done

  ssh_output="$(ssh -T -o BatchMode=yes -o StrictHostKeyChecking=accept-new -o ConnectTimeout=8 git@github.com 2>&1 || true)"
  if [[ "$has_key" == "true" && "$ssh_output" =~ "GitHub" ]]; then
    print_result "SSH access working" "PASS" "handshake con GitHub exitoso"
  else
    reason="sin llave SSH local o handshake fallido"
    [[ -n "$ssh_output" ]] && reason="$reason: $(echo "$ssh_output" | tr '\n' ' ' | cut -c1-160)"
    print_result "SSH access working" "FAIL" "$reason"
  fi
else
  print_result "SSH access working" "FAIL" "ssh no está instalado"
fi

# 7) Ability to pull from GitHub
if command -v git >/dev/null 2>&1; then
  tmp_dir="$(mktemp -d)"
  if git ls-remote https://github.com/git/git.git >/dev/null 2>&1; then
    print_result "Ability to pull from GitHub" "PASS" "acceso a repositorio remoto OK"
  else
    print_result "Ability to pull from GitHub" "FAIL" "no se pudo consultar GitHub vía git ls-remote"
  fi
  rm -rf "$tmp_dir"
else
  print_result "Ability to pull from GitHub" "FAIL" "git no está instalado"
fi

# 8) Port availability (80, 443, 3000)
port_fail=0
port_detail=""

check_port_in_use() {
  local p="$1"
  if command -v ss >/dev/null 2>&1; then
    ss -ltn "( sport = :${p} )" 2>/dev/null | tail -n +2 | grep -q .
    return $?
  fi

  if command -v netstat >/dev/null 2>&1; then
    netstat -ltn 2>/dev/null | awk '{print $4}' | grep -E "[:.]${p}$" -q
    return $?
  fi

  if command -v lsof >/dev/null 2>&1; then
    lsof -nP -iTCP:"${p}" -sTCP:LISTEN >/dev/null 2>&1
    return $?
  fi

  return 2
}

port_check_supported=1
for port in 80 443 3000; do
  if check_port_in_use "$port"; then
    port_fail=1
    port_detail+="${port}:ocupado "
  else
    rc=$?
    if [[ $rc -eq 2 ]]; then
      port_check_supported=0
      port_detail+="${port}:no-verificable "
    else
      port_detail+="${port}:libre "
    fi
  fi
done

if [[ $port_check_supported -eq 0 ]]; then
  print_result "Port availability (80,443,3000)" "FAIL" "sin herramientas ss/netstat/lsof (${port_detail})"
elif [[ $port_fail -eq 0 ]]; then
  print_result "Port availability (80,443,3000)" "PASS" "${port_detail}"
else
  print_result "Port availability (80,443,3000)" "FAIL" "${port_detail}"
fi

# 9) Disk space (umbral: >= 5GB libres en /)
avail_kb="$(df -Pk / | awk 'NR==2 {print $4}')"
if [[ -n "$avail_kb" ]]; then
  avail_gb=$((avail_kb / 1024 / 1024))
  if [[ $avail_gb -ge 5 ]]; then
    print_result "Disk space" "PASS" "${avail_gb}GB libres en /"
  else
    print_result "Disk space" "FAIL" "${avail_gb}GB libres en / (mínimo recomendado: 5GB)"
  fi
else
  print_result "Disk space" "FAIL" "no se pudo medir espacio en disco"
fi

# 10) Memory (umbral: >= 2GB RAM)
mem_kb="$(awk '/MemTotal/ {print $2}' /proc/meminfo 2>/dev/null || true)"
if [[ -n "$mem_kb" ]]; then
  mem_gb=$((mem_kb / 1024 / 1024))
  if [[ $mem_gb -ge 2 ]]; then
    print_result "Memory" "PASS" "${mem_gb}GB RAM detectada"
  else
    print_result "Memory" "FAIL" "${mem_gb}GB RAM detectada (mínimo recomendado: 2GB)"
  fi
else
  print_result "Memory" "FAIL" "no se pudo leer /proc/meminfo"
fi

echo "----------------------------------------"
echo "TOTAL PASS: ${pass_count}"
echo "TOTAL FAIL: ${fail_count}"

if [[ $fail_count -gt 0 ]]; then
  exit 1
fi

exit 0
