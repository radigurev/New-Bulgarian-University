<?php
declare(strict_types=1);

namespace App\Core;

final class Csrf
{
    public function __construct(private Session $session) {}

    public function token(): string
    {
        $token = (string)$this->session->get('csrf', '');
        if ($token === '') {
            $token = bin2hex(random_bytes(32));
            $this->session->set('csrf', $token);
        }
        return $token;
    }

    public function validate(?string $token): void
    {
        $stored = (string)$this->session->get('csrf', '');
        if ($stored === '' || $token === null || !hash_equals($stored, $token)) {
            http_response_code(400);
            exit('Invalid CSRF token');
        }
    }
}
