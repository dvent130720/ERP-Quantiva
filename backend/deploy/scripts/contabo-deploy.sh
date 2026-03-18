#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="${PROJECT_ROOT:-/var/www/ERP-Quantiva}"
BACKEND_DIR="$PROJECT_ROOT/backend"
NGINX_AVAILABLE="/etc/nginx/sites-available/quantiva-api.conf"
NGINX_ENABLED="/etc/nginx/sites-enabled/quantiva-api.conf"
NGINX_SOURCE="$BACKEND_DIR/deploy/nginx/quantiva-api.conf"

log() {
  printf '\n[%s] %s\n' "$(date '+%Y-%m-%d %H:%M:%S')" "$1"
}

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || {
    echo "Falta el comando requerido: $1" >&2
    exit 1
  }
}

require_cmd git
require_cmd docker
require_cmd nginx
require_cmd curl

if [ ! -d "$PROJECT_ROOT/.git" ]; then
  echo "No encontré el repositorio en $PROJECT_ROOT" >&2
  exit 1
fi

if [ ! -f "$BACKEND_DIR/.env" ]; then
  echo "Falta $BACKEND_DIR/.env. Cópialo desde .env.example y completa secretos." >&2
  exit 1
fi

log "Actualizando repositorio"
git -C "$PROJECT_ROOT" pull --ff-only

log "Levantando stack Docker"
docker compose -f "$BACKEND_DIR/docker-compose.yml" --env-file "$BACKEND_DIR/.env" up -d --build

log "Validando salud local"
for _ in $(seq 1 20); do
  if curl -fsS http://127.0.0.1:8080/health >/dev/null; then
    break
  fi
  sleep 3
done
curl -fsS http://127.0.0.1:8080/health >/dev/null

if [ -f "$NGINX_SOURCE" ]; then
  log "Instalando configuración Nginx"
  sudo cp "$NGINX_SOURCE" "$NGINX_AVAILABLE"
  if [ ! -L "$NGINX_ENABLED" ]; then
    sudo ln -sf "$NGINX_AVAILABLE" "$NGINX_ENABLED"
  fi
  sudo nginx -t
  sudo systemctl reload nginx
fi

log "Estado actual"
docker compose -f "$BACKEND_DIR/docker-compose.yml" --env-file "$BACKEND_DIR/.env" ps
log "Despliegue finalizado"
