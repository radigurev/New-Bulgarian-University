<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Util;
use App\Services\{DealService, CompanyService};
use App\Repositories\ContactRepository;

final class DealController extends Controller
{
    public function indexAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $stage = (string)($_GET['stage'] ?? '');
        $deals = $this->c->get(DealService::class)->list($userId, $stage);

        $this->render('deal/index', compact('deals','stage'));
    }

    public function createAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $companies = $this->c->get(CompanyService::class)->dropdown($userId);
        $contacts = $this->c->get(ContactRepository::class)->dropdown($userId);

        $prefCompanyId = Util::int($_GET['company_id'] ?? null);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $this->c->get(DealService::class)->create($userId, $_POST);
                $this->flash->set('ok', 'Deal created.');
                Util::redirect('deal/index');
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('deal/create', compact('companies','contacts','prefCompanyId','error'));
    }

    public function editAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_GET['id'] ?? 0);

        $svc = $this->c->get(DealService::class);
        $deal = $svc->get($id, $userId);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $svc->update($id, $userId, $_POST);
                $this->flash->set('ok', 'Deal updated.');
                Util::redirect('deal/index');
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('deal/edit', compact('deal','error'));
    }

    public function deleteAction(): void
    {
        $this->requireLogin();
        $this->csrf->validate($_POST['csrf'] ?? null);

        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_POST['id'] ?? 0);

        $this->c->get(DealService::class)->delete($id, $userId);
        $this->flash->set('ok', 'Deal deleted.');
        Util::redirect('deal/index');
    }
}
