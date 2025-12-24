<?php
declare(strict_types=1);

namespace App\Services;

use App\Core\Session;
use App\Repositories\UserRepository;

final class AuthService
{
    public function __construct(private Session $session, private UserRepository $users) {}

    public function isLoggedIn(): bool
    {
        return (int)$this->session->get('user_id', 0) > 0;
    }

    public function currentUser(): ?array
    {
        $id = (int)$this->session->get('user_id', 0);
        if ($id <= 0) return null;
        return [
            'id' => $id,
            'full_name' => (string)$this->session->get('full_name', ''),
        ];
    }

    public function login(string $email, string $password): bool
    {
        $email = trim($email);
        if ($email === '' || $password === '') return false;

        $u = $this->users->findByEmail($email);
        if (!$u) return false;

        if (!password_verify($password, (string)$u['password_hash'])) return false;

        $this->session->set('user_id', (int)$u['id']);
        $this->session->set('full_name', (string)$u['full_name']);
        return true;
    }

    public function logout(): void
    {
        $this->session->destroy();
    }

    public function initAdmin(): void
    {
        $email = 'admin@uni.local';
        if ($this->users->existsEmail($email)) return;

        $hash = password_hash('admin123', PASSWORD_DEFAULT);
        $this->users->create($email, $hash, 'Admin User');
    }
}
