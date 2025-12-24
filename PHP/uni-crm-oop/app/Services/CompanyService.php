<?php
declare(strict_types=1);

namespace App\Services;

use App\Repositories\CompanyRepository;

final class CompanyService
{
    public function __construct(private CompanyRepository $companies) {}

    public function list(int $userId, string $q = ''): array
    {
        return $this->companies->searchAll($userId, $q);
    }

    public function dropdown(int $userId): array
    {
        return $this->companies->dropdown($userId);
    }

    public function get(int $id, int $userId): array
    {
        $c = $this->companies->getById($id, $userId);
        if (!$c) throw new \RuntimeException('Company not found.');
        return $c;
    }

    public function create(int $userId, array $d): int
    {
        $name = trim((string)($d['name'] ?? ''));
        if ($name === '') throw new \RuntimeException('Company name is required.');

        $payload = [
            'name' => $name,
            'vat_number' => trim((string)($d['vat_number'] ?? '')) ?: null,
            'phone' => trim((string)($d['phone'] ?? '')) ?: null,
            'email' => trim((string)($d['email'] ?? '')) ?: null,
            'address' => trim((string)($d['address'] ?? '')) ?: null,
            'website' => trim((string)($d['website'] ?? '')) ?: null,
        ];

        return $this->companies->create($userId, $payload);
    }

    public function update(int $id, int $userId, array $d): void
    {
        $name = trim((string)($d['name'] ?? ''));
        if ($name === '') throw new \RuntimeException('Company name is required.');

        $payload = [
            'name' => $name,
            'vat_number' => trim((string)($d['vat_number'] ?? '')) ?: null,
            'phone' => trim((string)($d['phone'] ?? '')) ?: null,
            'email' => trim((string)($d['email'] ?? '')) ?: null,
            'address' => trim((string)($d['address'] ?? '')) ?: null,
            'website' => trim((string)($d['website'] ?? '')) ?: null,
        ];

        $this->companies->update($id, $userId, $payload);
    }

    public function delete(int $id, int $userId): void
    {
        $this->companies->delete($id, $userId);
    }
}
