<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Util;
use App\Services\NoteService;

final class NoteController extends Controller
{
    public function createForCompanyAction(): void
    {
        $this->requireLogin();
        $this->csrf->validate($_POST['csrf'] ?? null);

        $userId = (int)$this->auth->currentUser()['id'];
        $companyId = (int)($_POST['company_id'] ?? 0);

        try {
            $body = (string)($_POST['body'] ?? '');
            $this->c->get(NoteService::class)->create($userId, $companyId, $body);
            $this->flash->set('ok', 'Note added.');
        } catch (\Throwable $ex) {
            $this->flash->set('err', $ex->getMessage());
        }

        Util::redirect('company/view&id=' . $companyId);
    }
}
