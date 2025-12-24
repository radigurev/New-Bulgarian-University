<?php
declare(strict_types=1);

namespace App\Repositories;

use App\Core\Database;

final class UserRepository
{
    public function __construct(private Database $db) {}

    public function findByEmail(string $email): ?array
    {
        $st = $this->db->pdo()->prepare('SELECT * FROM users WHERE email=?');
        $st->execute([$email]);
        $u = $st->fetch();
        return $u ?: null;
    }

    public function existsEmail(string $email): bool
    {
        return (bool)$this->db->scalar('SELECT id FROM users WHERE email=?', [$email]);
    }

    public function create(string $email, string $passwordHash, string $fullName): int
    {
        $st = $this->db->pdo()->prepare('INSERT INTO users (email, password_hash, full_name) VALUES (?,?,?)');
        $st->execute([$email, $passwordHash, $fullName]);
        return (int)$this->db->pdo()->lastInsertId();
    }
}
