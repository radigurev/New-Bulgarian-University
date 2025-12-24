# Uni CRM (OOP, no framework)

## Requirements
- PHP 8.0+ (PDO MySQL enabled)
- MySQL / MariaDB
- Web server or PHP built-in server

## Setup
1) Import DB:
- Import `schema.sql` (creates database `uni_crm`)

2) Configure DB:
- Edit `app/Config/config.php`

3) Run:
- Built-in server:
  php -S localhost:8000 -t public

4) Create admin and login:
- Open: /?r=auth/initAdmin
- Login: admin@uni.local / admin123

## Routes
- dashboard/index
- auth/login, auth/logout, auth/initAdmin
- company/index, company/create, company/edit&id=, company/view&id=, company/delete
- contact/index, contact/create, contact/edit&id=, contact/delete
- deal/index, deal/create, deal/edit&id=, deal/delete
- task/index, task/create, task/edit&id=, task/delete
- note/createForCompany (POST)
