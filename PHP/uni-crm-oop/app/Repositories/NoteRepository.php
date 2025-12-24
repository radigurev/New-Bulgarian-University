<?php
declare(strict_types=1);

namespace App\Repositories;

use App\Core\Database;

final class NoteRepository
{
    public function __construct(private Database $db) {}

    public function forCompany(int $userId, int $companyId): array
    {
        $st = $this->db->pdo()->prepare('
            SELECT n.*, u.full_name
            FROM notes n
            JOIN users u ON u.id=n.owner_user_id
            JOIN companies co ON co.id=n.company_id
            WHERE n.company_id=? AND co.owner_user_id=?
            ORDER BY n.created_at DESC
        ');
        $st->execute([$companyId, $userId]);
        return $st->fetchAll();
    }

    public function create(int $userId, int $companyId, string $body): int
    {
        $st = $this->db->pdo()->prepare('INSERT INTO notes (owner_user_id, company_id, body) VALUES (?, ?, ?)');
        $st->execute([$userId, $companyId, $body]);
        return (int)$this->db->pdo()->lastInsertId();
    }
}
