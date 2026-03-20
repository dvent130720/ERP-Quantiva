CREATE TABLE IF NOT EXISTS companies (
    id UUID PRIMARY KEY,
    tax_id VARCHAR(32) NOT NULL UNIQUE,
    legal_name VARCHAR(200) NOT NULL,
    trade_name VARCHAR(200),
    email VARCHAR(200) NOT NULL,
    phone VARCHAR(32),
    address VARCHAR(300),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at_utc TIMESTAMPTZ NOT NULL,
    updated_at_utc TIMESTAMPTZ NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_companies_tax_id ON companies (tax_id);
CREATE INDEX IF NOT EXISTS idx_companies_legal_name ON companies (legal_name);

CREATE TABLE IF NOT EXISTS app_users (
    id UUID PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role VARCHAR(50) NOT NULL,
    created_at_utc TIMESTAMPTZ NOT NULL
);

INSERT INTO app_users (id, username, password_hash, role, created_at_utc)
SELECT
    '11111111-1111-1111-1111-111111111111',
    'admin',
    '100000.A6Qx5fzgH1x/AZueF6XEdw==.JsQ3DFXLp+O6fCXeJGI9hOT0dim5LnCNtXRfknf/yVs=',
    'Administrator',
    NOW()
WHERE NOT EXISTS (
    SELECT 1
    FROM app_users
    WHERE username = 'admin'
);
