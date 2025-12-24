<?php
declare(strict_types=1);

namespace App\Services;

use App\Repositories\ContactRepository;
use App\Repositories\CompanyRepository;

final class ContactService
{
    public function __construct(private ContactRepository $contacts, private CompanyRepository $companies) {}

    public function list(int $userId, ?int $companyId): array
    {
        return $this->contacts->list($userId, $companyId);
    }

    public function get(int $id, int $userId): array
    {
        $c = $this->contacts->getById($id, $userId);
        if (!$c) throw new \RuntimeException('Contact not found.');
        return $c;
    }

    public function create(int $userId, array $d): int
    {
        $companyId = (int)($d['company_id'] ?? 0);
        if ($companyId <= 0) throw new \RuntimeException('Company is required.');

        if (!$this->companies->getById($companyId, $userId)) throw new \RuntimeException('Invalid company.');

        $first = trim((string)($d['first_name'] ?? ''));
        $last  = trim((string)($d['last_name'] ?? ''));
        if ($first === '' || $last === '') throw new \RuntimeException('First and last name are required.');

        return $this->contacts->create($companyId, [
            'first_name' => $first,
            'last_name' => $last,
            'phone' => trim((string)($d['phone'] ?? '')) ?: null,
            'email' => trim((string)($d['email'] ?? '')) ?: null,
            'position' => trim((string)($d['position'] ?? '')) ?: null,
        ]);
    }

    public function update(int $id, int $userId, array $d): void
    {
        $this->get($id, $userId);

        $first = trim((string)($d['first_name'] ?? ''));
        $last  = trim((string)($d['last_name'] ?? ''));
        if ($first === '' || $last === '') throw new \RuntimeException('First and last name are required.');

        $this->contacts->update($id, [
            'first_name' => $first,
            'last_name' => $last,
            'phone' => trim((string)($d['phone'] ?? '')) ?: null,
            'email' => trim((string)($d['email'] ?? '')) ?: null,
            'position' => trim((string)($d['position'] ?? '')) ?: null,
        ]);
    }

    public function delete(int $id, int $userId): void
    {
        $this->get($id, $userId);
        $this->contacts->delete($id);
    }
}
