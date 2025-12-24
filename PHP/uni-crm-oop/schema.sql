CREATE DATABASE IF NOT EXISTS uni_crm CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE uni_crm;

CREATE TABLE users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  email VARCHAR(190) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  full_name VARCHAR(190) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE companies (
  id INT AUTO_INCREMENT PRIMARY KEY,
  owner_user_id INT NOT NULL,
  name VARCHAR(190) NOT NULL,
  vat_number VARCHAR(50) NULL,
  phone VARCHAR(50) NULL,
  email VARCHAR(190) NULL,
  address VARCHAR(255) NULL,
  website VARCHAR(190) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX (owner_user_id),
  CONSTRAINT fk_companies_owner FOREIGN KEY (owner_user_id) REFERENCES users(id)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE contacts (
  id INT AUTO_INCREMENT PRIMARY KEY,
  company_id INT NOT NULL,
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  phone VARCHAR(50) NULL,
  email VARCHAR(190) NULL,
  position VARCHAR(190) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX (company_id),
  CONSTRAINT fk_contacts_company FOREIGN KEY (company_id) REFERENCES companies(id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE deals (
  id INT AUTO_INCREMENT PRIMARY KEY,
  company_id INT NOT NULL,
  contact_id INT NULL,
  owner_user_id INT NOT NULL,
  title VARCHAR(190) NOT NULL,
  amount DECIMAL(12,2) NULL,
  stage ENUM('New','Qualified','Proposal','Won','Lost') NOT NULL DEFAULT 'New',
  expected_close_date DATE NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX (company_id),
  INDEX (contact_id),
  INDEX (owner_user_id),
  CONSTRAINT fk_deals_company FOREIGN KEY (company_id) REFERENCES companies(id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_deals_contact FOREIGN KEY (contact_id) REFERENCES contacts(id)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT fk_deals_owner FOREIGN KEY (owner_user_id) REFERENCES users(id)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE tasks (
  id INT AUTO_INCREMENT PRIMARY KEY,
  owner_user_id INT NOT NULL,
  company_id INT NULL,
  deal_id INT NULL,
  title VARCHAR(190) NOT NULL,
  due_date DATE NULL,
  status ENUM('Open','Done') NOT NULL DEFAULT 'Open',
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX (owner_user_id),
  INDEX (company_id),
  INDEX (deal_id),
  CONSTRAINT fk_tasks_owner FOREIGN KEY (owner_user_id) REFERENCES users(id)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_tasks_company FOREIGN KEY (company_id) REFERENCES companies(id)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT fk_tasks_deal FOREIGN KEY (deal_id) REFERENCES deals(id)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE notes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  owner_user_id INT NOT NULL,
  company_id INT NULL,
  contact_id INT NULL,
  deal_id INT NULL,
  body TEXT NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX (owner_user_id),
  INDEX (company_id),
  INDEX (contact_id),
  INDEX (deal_id),
  CONSTRAINT fk_notes_owner FOREIGN KEY (owner_user_id) REFERENCES users(id)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_notes_company FOREIGN KEY (company_id) REFERENCES companies(id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_notes_contact FOREIGN KEY (contact_id) REFERENCES contacts(id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_notes_deal FOREIGN KEY (deal_id) REFERENCES deals(id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;
