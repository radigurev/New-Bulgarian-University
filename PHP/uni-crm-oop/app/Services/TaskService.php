<?php
declare(strict_types=1);

namespace App\Services;

use App\Repositories\TaskRepository;
use App\Repositories\CompanyRepository;
use App\Repositories\DealRepository;

final class TaskService
{
    public function __construct(
        private TaskRepository $tasks,
        private CompanyRepository $companies,
        private DealRepository $deals
    ) {}

    public function list(int $userId, string $status = ''): array
    {
        $status = trim($status);
        if ($status !== '' && !in_array($status, ['Open','Done'], true)) $status = '';
        return $this->tasks->list($userId, $status);
    }

    public function forCompany(int $userId, int $companyId): array
    {
        return $this->tasks->forCompany($userId, $companyId);
    }

    public function get(int $id, int $userId): array
    {
        $t = $this->tasks->getById($id, $userId);
        if (!$t) throw new \RuntimeException('Task not found.');
        return $t;
    }

    public function create(int $userId, array $d): int
    {
        $title = trim((string)($d['title'] ?? ''));
        if ($title === '') throw new \RuntimeException('Task title is required.');

        $status = (string)($d['status'] ?? 'Open');
        if (!in_array($status, ['Open','Done'], true)) $status = 'Open';

        $due = trim((string)($d['due_date'] ?? ''));
        $due = $due === '' ? null : $due;

        $companyId = trim((string)($d['company_id'] ?? ''));
        $companyId = $companyId === '' ? null : (int)$companyId;
        if ($companyId && !$this->companies->getById($companyId, $userId)) $companyId = null;

        $dealId = trim((string)($d['deal_id'] ?? ''));
        $dealId = $dealId === '' ? null : (int)$dealId;
        if ($dealId && !$this->deals->getById($dealId, $userId)) $dealId = null;

        return $this->tasks->create($userId, [
            'title' => $title,
            'due_date' => $due,
            'status' => $status,
            'company_id' => $companyId,
            'deal_id' => $dealId,
        ]);
    }

    public function update(int $id, int $userId, array $d): void
    {
        $this->get($id, $userId);

        $title = trim((string)($d['title'] ?? ''));
        if ($title === '') throw new \RuntimeException('Task title is required.');

        $status = (string)($d['status'] ?? 'Open');
        if (!in_array($status, ['Open','Done'], true)) $status = 'Open';

        $due = trim((string)($d['due_date'] ?? ''));
        $due = $due === '' ? null : $due;

        $companyId = trim((string)($d['company_id'] ?? ''));
        $companyId = $companyId === '' ? null : (int)$companyId;
        if ($companyId && !$this->companies->getById($companyId, $userId)) $companyId = null;

        $dealId = trim((string)($d['deal_id'] ?? ''));
        $dealId = $dealId === '' ? null : (int)$dealId;
        if ($dealId && !$this->deals->getById($dealId, $userId)) $dealId = null;

        $this->tasks->update($id, $userId, [
            'title' => $title,
            'due_date' => $due,
            'status' => $status,
            'company_id' => $companyId,
            'deal_id' => $dealId,
        ]);
    }

    public function delete(int $id, int $userId): void
    {
        $this->tasks->delete($id, $userId);
    }
}
