<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Database;

final class DashboardController extends Controller
{
    public function indexAction(): void
    {
        $this->requireLogin();
        $userId = (int)$this->auth->currentUser()['id'];

        $db = $this->c->get(Database::class);

        $companyCount = (int)$db->scalar('SELECT COUNT(*) FROM companies WHERE owner_user_id=?', [$userId]);
        $dealCount    = (int)$db->scalar('SELECT COUNT(*) FROM deals WHERE owner_user_id=?', [$userId]);
        $openTasks    = (int)$db->scalar('SELECT COUNT(*) FROM tasks WHERE owner_user_id=? AND status="Open"', [$userId]);

        $this->render('dashboard/index', compact('companyCount','dealCount','openTasks'));
    }
}
