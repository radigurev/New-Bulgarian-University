<?php
declare(strict_types=1);

namespace App\Services;

use App\Repositories\DealRepository;
use App\Repositories\CompanyRepository;

final class DealService
{
    private array $stages = ['New','Qualified','Proposal','Won','Lost'];

    public function __construct(private DealRepository $deals, private CompanyRepository $companies) {}

    public function stages(): array { return $this->stages; }

    public function list(int $userId, string $stage = ''): array
    {
        $stage = trim($stage);
        if ($stage !== '' && !in_array($stage, $this->stages, true)) $stage = '';
        return $this->deals->list($userId, $stage);
    }

    public function dropdown(int $userId): array
    {
        return $this->deals->dropdown($userId);
    }

    public function forCompany(int $userId, int $companyId): array
    {
        return $this->deals->forCompany($userId, $companyId);
    }

    public function get(int $id, int $userId): array
    {
        $d = $this->deals->getById($id, $userId);
        if (!$d) throw new \RuntimeException('Deal not found.');
        return $d;
    }

    public function create(int $userId, array $d): int
    {
        $companyId = (int)($d['company_id'] ?? 0);
        if ($companyId <= 0) throw new \RuntimeException('Company is required.');
        if (!$this->companies->getById($companyId, $userId)) throw new \RuntimeException('Invalid company.');

        $title = trim((string)($d['title'] ?? ''));
        if ($title === '') throw new \RuntimeException('Deal title is required.');

        $stage = (string)($d['stage'] ?? 'New');
        if (!in_array($stage, $this->stages, true)) $stage = 'New';

        $amount = trim((string)($d['amount'] ?? ''));
        $amount = $amount === '' ? null : $amount;

        $expected = trim((string)($d['expected_close_date'] ?? ''));
        $expected = $expected === '' ? null : $expected;

        $contactId = trim((string)($d['contact_id'] ?? ''));
        $contactId = $contactId === '' ? null : (int)$contactId;

        return $this->deals->create($userId, [
            'company_id' => $companyId,
            'contact_id' => $contactId,
            'title' => $title,
            'amount' => $amount,
            'stage' => $stage,
            'expected_close_date' => $expected,
        ]);
    }

    public function update(int $id, int $userId, array $d): void
    {
        $this->get($id, $userId);

        $title = trim((string)($d['title'] ?? ''));
        if ($title === '') throw new \RuntimeException('Deal title is required.');

        $stage = (string)($d['stage'] ?? 'New');
        if (!in_array($stage, $this->stages, true)) $stage = 'New';

        $amount = trim((string)($d['amount'] ?? ''));
        $amount = $amount === '' ? null : $amount;

        $expected = trim((string)($d['expected_close_date'] ?? ''));
        $expected = $expected === '' ? null : $expected;

        $this->deals->update($id, $userId, [
            'title' => $title,
            'amount' => $amount,
            'stage' => $stage,
            'expected_close_date' => $expected,
        ]);
    }

    public function delete(int $id, int $userId): void
    {
        $this->deals->delete($id, $userId);
    }
}
