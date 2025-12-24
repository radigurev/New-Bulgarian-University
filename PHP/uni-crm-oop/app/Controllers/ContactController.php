<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Util;
use App\Services\{ContactService, CompanyService};

final class ContactController extends Controller
{
    public function indexAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];
        $companyId = Util::int($_GET['company_id'] ?? null);

        $companies = $this->c->get(CompanyService::class)->dropdown($userId);
        $contacts = $this->c->get(ContactService::class)->list($userId, $companyId);

        $this->render('contact/index', compact('contacts','companies','companyId'));
    }

    public function createAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $companies = $this->c->get(CompanyService::class)->dropdown($userId);
        $prefCompanyId = Util::int($_GET['company_id'] ?? null);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $this->c->get(ContactService::class)->create($userId, $_POST);
                $this->flash->set('ok', 'Contact created.');
                Util::redirect('company/view&id=' . (int)$_POST['company_id']);
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('contact/create', compact('companies','prefCompanyId','error'));
    }

    public function editAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_GET['id'] ?? 0);

        $svc = $this->c->get(ContactService::class);
        $contact = $svc->get($id, $userId);

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            try {
                $svc->update($id, $userId, $_POST);
                $this->flash->set('ok', 'Contact updated.');
                Util::redirect('company/view&id=' . (int)$contact['company_id']);
            } catch (\Throwable $ex) {
                $error = $ex->getMessage();
            }
        }

        $this->render('contact/edit', compact('contact','error'));
    }

    public function deleteAction(): void
    {
        $this->requireLogin();
        $this->csrf->validate($_POST['csrf'] ?? null);

        $userId = (int)$this->auth->currentUser()['id'];
        $id = (int)($_POST['id'] ?? 0);

        $this->c->get(ContactService::class)->delete($id, $userId);
        $this->flash->set('ok', 'Contact deleted.');
        Util::redirect('contact/index');
    }
}
