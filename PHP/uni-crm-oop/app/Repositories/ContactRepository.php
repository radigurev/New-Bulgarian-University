<?php
declare(strict_types=1);

namespace App\Repositories;

use App\Core\Database;

final class ContactRepository
{
    public function __construct(private Database $db) {}

    public function list(int $userId, ?int $companyId): array
    {
        if ($companyId) {
            $st = $this->db->pdo()->prepare('
                SELECT c.*, co.name AS company_name
                FROM contacts c
                JOIN companies co ON co.id=c.company_id
                WHERE co.owner_user_id=? AND c.company_id=?
                ORDER BY c.created_at DESC
            ');
            $st->execute([$userId, $companyId]);
            return $st->fetchAll();
        }

        $st = $this->db->pdo()->prepare('
            SELECT c.*, co.name AS company_name
            FROM contacts c
            JOIN companies co ON co.id=c.company_id
            WHERE co.owner_user_id=?
            ORDER BY c.created_at DESC
        ');
        $st->execute([$userId]);
        return $st->fetchAll();
    }

    public function dropdown(int $userId): array
    {
        $st = $this->db->pdo()->prepare('
            SELECT c.id, CONCAT(c.first_name," ",c.last_name) AS name, co.name AS company_name
            FROM contacts c
            JOIN companies co ON co.id=c.company_id
            WHERE co.owner_user_id=?
            ORDER BY co.name ASC, c.last_name ASC
        ');
        $st->execute([$userId]);
        return $st->fetchAll();
    }

    public function getById(int $id, int $userId): ?array
    {
        $st = $this->db->pdo()->prepare('
            SELECT c.*, co.name AS company_name
            FROM contacts c
            JOIN companies co ON co.id=c.company_id
            WHERE c.id=? AND co.owner_user_id=?
        ');
        $st->execute([$id, $userId]);
        $r = $st->fetch();
        return $r ?: null;
    }

    public function create(int $companyId, array $d): int
    {
        $st = $this->db->pdo()->prepare('
            INSERT INTO contacts (company_id, first_name, last_name, phone, email, position)
            VALUES (?, ?, ?, ?, ?, ?)
        ');
        $st->execute([
            $companyId,
            $d['first_name'],
            $d['last_name'],
            $d['phone'] ?? null,
            $d['email'] ?? null,
            $d['position'] ?? null,
        ]);
        return (int)$this->db->pdo()->lastInsertId();
    }

    public function update(int $id, array $d): void
    {
        $st = $this->db->pdo()->prepare('
            UPDATE contacts
            SET first_name=?, last_name=?, phone=?, email=?, position=?
            WHERE id=?
        ');
        $st->execute([
            $d['first_name'],
            $d['last_name'],
            $d['phone'] ?? null,
            $d['email'] ?? null,
            $d['position'] ?? null,
            $id
        ]);
    }

    public function delete(int $id): void
    {
        $st = $this->db->pdo()->prepare('DELETE FROM contacts WHERE id=?');
        $st->execute([$id]);
    }
}
