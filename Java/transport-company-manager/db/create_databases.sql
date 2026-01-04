-- Optional helper script (Hibernate can create/update tables automatically)

CREATE DATABASE IF NOT EXISTS transport_company
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE DATABASE IF NOT EXISTS transport_company_test
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Example user (change password)
-- CREATE USER IF NOT EXISTS 'transport_user'@'localhost' IDENTIFIED BY 'transport_pass';
-- GRANT ALL PRIVILEGES ON transport_company.* TO 'transport_user'@'localhost';
-- GRANT ALL PRIVILEGES ON transport_company_test.* TO 'transport_user'@'localhost';
-- FLUSH PRIVILEGES;
