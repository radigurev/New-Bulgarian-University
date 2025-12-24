<?php
declare(strict_types=1);

namespace App\Repositories;

use App\Core\Database;

final class DealRepository
{
    public function __construct(private Database $db) {}

    public function list(int $userId, string $stage = ''): array
    {
        if ($stage === '') {
            $st = $this->db->pdo()->prepare('
                SELECT d.*, co.name AS company_name
                FROM deals d
                JOIN companies co ON co.id=d.company_id
                WHERE d.owner_user_id=?
                ORDER BY d.created_at DESC
            ');
            $st->execute([$userId]);
            return $st->fetchAll();
        }

        $st = $this->db->pdo()->prepare('
            SELECT d.*, co.name AS company_name
            FROM deals d
            JOIN companies co ON co.id=d.company_id
            WHERE d.owner_user_id=? AND d.stage=?
            ORDER BY d.created_at DESC
        ');
        $st->execute([$userId, $stage]);
        return $st->fetchAll();
    }

    public function forCompany(int $userId, int $companyId): array
    {
        $st = $this->db->pdo()->prepare('
            SELECT d.*
            FROM deals d
            JOIN companies co ON co.id=d.company_id
            WHERE d.owner_user_id=? AND d.company_id=? AND co.owner_user_id=?
            ORDER BY d.created_at DESC
        ');
        $st->execute([$userId, $companyId, $userId]);
        return $st->fetchAll();
    }

    public function dropdown(int $userId): array
    {
        $st = $this->db->pdo()->prepare('
            SELECT d.id, d.title, co.name AS company_name
            FROM deals d
            JOIN companies co ON co.id=d.company_id
            WHERE d.owner_user_id=?
            ORDER BY d.created_at DESC
        ');
        $st->execute([$userId]);
        return $st->fetchAll();
    }

    public function getById(int $id, int $userId): ?array
    {
        $st = $this->db->pdo()->prepare('
            SELECT d.*, co.name AS company_name
            FROM deals d
            JOIN companies co ON co.id=d.company_id
            WHERE d.id=? AND d.owner_user_id=? AND co.owner_user_id=?
        ');
        $st->execute([$id, $userId, $userId]);
        $r = $st->fetch();
        return $r ?: null;
    }

    public function create(int $userId, array $d): int
    {
        $st = $this->db->pdo()->prepare('
            INSERT INTO deals (company_id, contact_id, owner_user_id, title, amount, stage, expected_close_date)
            VALUES (?, ?, ?, ?, ?, ?, ?)
        ');
        $st->execute([
            $d['company_id'],
            $d['contact_id'] ?? null,
            $userId,
            $d['title'],
            $d['amount'] ?? null,
            $d['stage'],
            $d['expected_close_date'] ?? null
        ]);
        return (int)$this->db->pdo()->lastInsertId();
    }

    public function update(int $id, int $userId, array $d): void
    {
        $st = $this->db->pdo()->prepare('
            UPDATE deals
            SET title=?, amount=?, stage=?, expected_close_date=?
            WHERE id=? AND owner_user_id=?
        ');
        $st->execute([
            $d['title'],
            $d['amount'] ?? null,
            $d['stage'],
            $d['expected_close_date'] ?? null,
            $id, $userId
        ]);
    }

    public function delete(int $id, int $userId): void
    {
        $st = $this->db->pdo()->prepare('DELETE FROM deals WHERE id=? AND owner_user_id=?');
        $st->execute([$id, $userId]);
    }
}
