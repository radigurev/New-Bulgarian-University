<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Util;
use App\Services\{CompanyService, ContactService, DealService, TaskService, NoteService};

final class CompanyController extends Controller
{
    public function indexAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $q = (string)($_GET['q'] ?? '');
        $companies = $this->c->get(CompanyService::class)->list($userId, $q);

        $this->render('company/index', compact('companies','q'));
    }

    public function createAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $id = $this->c->get(CompanyService::class)->create($userId, $_POST);
                $this->flash->set('ok', 'Company created.');
                Util::redirect('company/view&id=' . $id);
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('company/create', compact('error'));
    }

    public function editAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_GET['id'] ?? 0);

        $svc = $this->c->get(CompanyService::class);
        $company = $svc->get($id, $userId);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $svc->update($id, $userId, $_POST);
                $this->flash->set('ok', 'Company updated.');
                Util::redirect('company/view&id=' . $id);
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('company/edit', compact('company','error'));
    }

    public function deleteAction(): void
    {
        $this->requireLogin();
        $this->csrf->validate($_POST['csrf'] ?? null);

        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_POST['id'] ?? 0);

        $this->c->get(CompanyService::class)->delete($id, $userId);
        $this->flash->set('ok', 'Company deleted.');
        Util::redirect('company/index');
    }

    public function viewAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_GET['id'] ?? 0);

        $companySvc = $this->c->get(CompanyService::class);
        $contactSvc = $this->c->get(ContactService::class);
        $dealSvc = $this->c->get(DealService::class);
        $taskSvc = $this->c->get(TaskService::class);
        $noteSvc = $this->c->get(NoteService::class);

        $company = $companySvc->get($id, $userId);
        $contacts = $contactSvc->list($userId, $id);
        $deals = $dealSvc->forCompany($userId, $id);
        $tasks = $taskSvc->forCompany($userId, $id);
        $notes = $noteSvc->forCompany($userId, $id);

        $this->render('company/view', compact('company','contacts','deals','tasks','notes'));
    }
}
