<?php
declare(strict_types=1);

namespace App\Repositories;

use App\Core\Database;

final class CompanyRepository
{
    public function __construct(private Database $db) {}

    public function searchAll(int $userId, string $q = ''): array
    {
        $q = trim($q);
        if ($q === '') {
            $st = $this->db->pdo()->prepare('SELECT * FROM companies WHERE owner_user_id=? ORDER BY created_at DESC');
            $st->execute([$userId]);
            return $st->fetchAll();
        }
        $like = "%$q%";
        $st = $this->db->pdo()->prepare('
            SELECT * FROM companies
            WHERE owner_user_id=?
              AND (name LIKE ? OR email LIKE ? OR phone LIKE ? OR vat_number LIKE ?)
            ORDER BY created_at DESC
        ');
        $st->execute([$userId, $like, $like, $like, $like]);
        return $st->fetchAll();
    }

    public function dropdown(int $userId): array
    {
        $st = $this->db->pdo()->prepare('SELECT id,name FROM companies WHERE owner_user_id=? ORDER BY name ASC');
        $st->execute([$userId]);
        return $st->fetchAll();
    }

    public function getById(int $id, int $userId): ?array
    {
        $st = $this->db->pdo()->prepare('SELECT * FROM companies WHERE id=? AND owner_user_id=?');
        $st->execute([$id, $userId]);
        $r = $st->fetch();
        return $r ?: null;
    }

    public function create(int $userId, array $d): int
    {
        $st = $this->db->pdo()->prepare('
            INSERT INTO companies (owner_user_id, name, vat_number, phone, email, address, website)
            VALUES (?, ?, ?, ?, ?, ?, ?)
        ');
        $st->execute([
            $userId,
            $d['name'],
            $d['vat_number'] ?? null,
            $d['phone'] ?? null,
            $d['email'] ?? null,
            $d['address'] ?? null,
            $d['website'] ?? null,
        ]);
        return (int)$this->db->pdo()->lastInsertId();
    }

    public function update(int $id, int $userId, array $d): void
    {
        $st = $this->db->pdo()->prepare('
            UPDATE companies
            SET name=?, vat_number=?, phone=?, email=?, address=?, website=?
            WHERE id=? AND owner_user_id=?
        ');
        $st->execute([
            $d['name'],
            $d['vat_number'] ?? null,
            $d['phone'] ?? null,
            $d['email'] ?? null,
            $d['address'] ?? null,
            $d['website'] ?? null,
            $id, $userId
        ]);
    }

    public function delete(int $id, int $userId): void
    {
        $st = $this->db->pdo()->prepare('DELETE FROM companies WHERE id=? AND owner_user_id=?');
        $st->execute([$id, $userId]);
    }
}
