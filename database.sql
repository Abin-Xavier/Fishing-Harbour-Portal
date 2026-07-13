-- Coastal Fishing Harbour Portal — MySQL schema
-- Phase 3 of the project plan. Matches the fields used by the
-- frontend forms (register.html) and tables (market.html).

CREATE DATABASE IF NOT EXISTS fishing_harbour;
USE fishing_harbour;

CREATE TABLE IF NOT EXISTS boats (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    boat_name     VARCHAR(100) NOT NULL,
    owner         VARCHAR(100) NOT NULL,
    boat_number   VARCHAR(50)  NOT NULL UNIQUE,
    fishing_type  VARCHAR(50)  NOT NULL,
    contact       VARCHAR(20)  NOT NULL,
    created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS fish_details (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    fish_name   VARCHAR(100) NOT NULL,
    quantity_kg DECIMAL(10,2) NOT NULL,
    price_per_kg DECIMAL(10,2) NOT NULL,
    updated_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS users (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    name       VARCHAR(100) NOT NULL,
    email      VARCHAR(150) NOT NULL UNIQUE,
    password   VARCHAR(255) NOT NULL,  -- store a bcrypt/argon2 hash, never plain text
    role       VARCHAR(20) DEFAULT 'fisherman',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Holds the harbour-wide figures that aren't simple table counts.
-- The API treats this as a single-row table: active boats and
-- registered fishermen are computed live from `boats` and `users`.
CREATE TABLE IF NOT EXISTS harbour_status (
    id                 INT AUTO_INCREMENT PRIMARY KEY,
    todays_catch_tons  DECIMAL(6,2) NOT NULL DEFAULT 0,
    cold_storage_available TINYINT(1) NOT NULL DEFAULT 1,
    updated_at         TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Seed data matching the Fish Market page
INSERT INTO fish_details (fish_name, quantity_kg, price_per_kg) VALUES
('Tuna', 500, 450),
('Sardine', 800, 180),
('Mackerel', 300, 250),
('Pomfret', 150, 620),
('Prawns', 220, 540);

-- Seed the single harbour_status row matching the dashboard's starting figures
INSERT INTO harbour_status (todays_catch_tons, cold_storage_available) VALUES (2.5, 1);
