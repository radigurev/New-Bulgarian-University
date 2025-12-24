<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Util;
use App\Services\{TaskService, CompanyService, DealService};

final class TaskController extends Controller
{
    public function indexAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $status = (string)($_GET['status'] ?? '');
        $tasks = $this->c->get(TaskService::class)->list($userId, $status);

        $this->render('task/index', compact('tasks','status'));
    }

    public function createAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $companies = $this->c->get(CompanyService::class)->dropdown($userId);
        $deals = $this->c->get(DealService::class)->dropdown($userId);
        $prefCompanyId = Util::int($_GET['company_id'] ?? null);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $this->c->get(TaskService::class)->create($userId, $_POST);
                $this->flash->set('ok', 'Task created.');
                Util::redirect('task/index');
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('task/create', compact('companies','deals','prefCompanyId','error'));
    }

    public function editAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_GET['id'] ?? 0);

        $svc = $this->c->get(TaskService::class);
        $task = $svc->get($id, $userId);

        $companies = $this->c->get(CompanyService::class)->dropdown($userId);
        $deals = $this->c->get(DealService::class)->dropdown($userId);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $svc->update($id, $userId, $_POST);
                $this->flash->set('ok', 'Task updated.');
                Util::redirect('task/index');
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('task/edit', compact('task','companies','deals','error'));
    }

    public function deleteAction(): void
    {
        $this->requireLogin();
        $this->csrf->validate($_POST['csrf'] ?? null);

        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_POST['id'] ?? 0);

        $this->c->get(TaskService::class)->delete($id, $userId);
        $this->flash->set('ok', 'Task deleted.');
        Util::redirect('task/index');
    }
}
