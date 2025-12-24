<?php
declare(strict_types=1);

namespace App\Services;

use App\Repositories\NoteRepository;
use App\Repositories\CompanyRepository;

final class NoteService
{
    public function __construct(private NoteRepository $notes, private CompanyRepository $companies) {}

    public function forCompany(int $userId, int $companyId): array
    {
        return $this->notes->forCompany($userId, $companyId);
    }

    public function create(int $userId, int $companyId, string $body): int
    {
        $body = trim($body);
        if ($body === '') throw new \RuntimeException('Note text is required.');

        if (!$this->companies->getById($companyId, $userId)) throw new \RuntimeException('Invalid company.');
        return $this->notes->create($userId, $companyId, $body);
    }
}
