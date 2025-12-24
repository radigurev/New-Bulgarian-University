<?php
declare(strict_types=1);

namespace App\Repositories;

use App\Core\Database;

final class TaskRepository
{
    public function __construct(private Database $db) {}

    public function list(int $userId, string $status = ''): array
    {
        $sql = '
            SELECT t.*, co.name AS company_name, d.title AS deal_title
            FROM tasks t
            LEFT JOIN companies co ON co.id=t.company_id
            LEFT JOIN deals d ON d.id=t.deal_id
            WHERE t.owner_user_id=?
        ';
        $params = [$userId];

        if ($status !== '') {
            $sql .= ' AND t.status=?';
            $params[] = $status;
        }

        $sql .= ' ORDER BY (t.due_date IS NULL), t.due_date ASC, t.created_at DESC';

        $st = $this->db->pdo()->prepare($sql);
        $st->execute($params);
        return $st->fetchAll();
    }

    public function forCompany(int $userId, int $companyId): array
    {
        $st = $this->db->pdo()->prepare('
            SELECT t.*, d.title AS deal_title
            FROM tasks t
            LEFT JOIN deals d ON d.id=t.deal_id
            WHERE t.owner_user_id=? AND t.company_id=?
            ORDER BY (t.due_date IS NULL), t.due_date ASC, t.created_at DESC
        ');
        $st->execute([$userId, $companyId]);
        return $st->fetchAll();
    }

    public function getById(int $id, int $userId): ?array
    {
        $st = $this->db->pdo()->prepare('SELECT * FROM tasks WHERE id=? AND owner_user_id=?');
        $st->execute([$id, $userId]);
        $r = $st->fetch();
        return $r ?: null;
    }

    public function create(int $userId, array $d): int
    {
        $st = $this->db->pdo()->prepare('
            INSERT INTO tasks (owner_user_id, company_id, deal_id, title, due_date, status)
            VALUES (?, ?, ?, ?, ?, ?)
        ');
        $st->execute([
            $userId,
            $d['company_id'] ?? null,
            $d['deal_id'] ?? null,
            $d['title'],
            $d['due_date'] ?? null,
            $d['status'],
        ]);
        return (int)$this->db->pdo()->lastInsertId();
    }

    public function update(int $id, int $userId, array $d): void
    {
        $st = $this->db->pdo()->prepare('
            UPDATE tasks
            SET company_id=?, deal_id=?, title=?, due_date=?, status=?
            WHERE id=? AND owner_user_id=?
        ');
        $st->execute([
            $d['company_id'] ?? null,
            $d['deal_id'] ?? null,
            $d['title'],
            $d['due_date'] ?? null,
            $d['status'],
            $id, $userId
        ]);
    }

    public function delete(int $id, int $userId): void
    {
        $st = $this->db->pdo()->prepare('DELETE FROM tasks WHERE id=? AND owner_user_id=?');
        $st->execute([$id, $userId]);
    }
}
