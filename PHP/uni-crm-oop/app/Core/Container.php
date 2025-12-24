<?php
declare(strict_types=1);

namespace App\Core;

use App\Repositories\{UserRepository, CompanyRepository, ContactRepository, DealRepository, TaskRepository, NoteRepository};
use App\Services\{AuthService, CompanyService, ContactService, DealService, TaskService, NoteService};

final class Container
{
    private array $instances = [];

    public function __construct(private array $config) {}

    public function get(string $id): mixed
    {
        if (isset($this->instances[$id])) return $this->instances[$id];

        $obj = match ($id) {
            Database::class => new Database($this->config),
            Session::class => new Session(),
            Flash::class => new Flash($this->get(Session::class)),
            Csrf::class => new Csrf($this->get(Session::class)),
            View::class => new View(__DIR__ . '/../Views'),

            // Repositories
            UserRepository::class => new UserRepository($this->get(Database::class)),
            CompanyRepository::class => new CompanyRepository($this->get(Database::class)),
            ContactRepository::class => new ContactRepository($this->get(Database::class)),
            DealRepository::class => new DealRepository($this->get(Database::class)),
            TaskRepository::class => new TaskRepository($this->get(Database::class)),
            NoteRepository::class => new NoteRepository($this->get(Database::class)),

            // Services
            AuthService::class => new AuthService($this->get(Session::class), $this->get(UserRepository::class)),
            CompanyService::class => new CompanyService($this->get(CompanyRepository::class)),
            ContactService::class => new ContactService($this->get(ContactRepository::class), $this->get(CompanyRepository::class)),
            DealService::class => new DealService($this->get(DealRepository::class), $this->get(CompanyRepository::class)),
            TaskService::class => new TaskService($this->get(TaskRepository::class), $this->get(CompanyRepository::class), $this->get(DealRepository::class)),
            NoteService::class => new NoteService($this->get(NoteRepository::class), $this->get(CompanyRepository::class)),

            default => throw new \RuntimeException("Container: unknown service $id")
        };

        $this->instances[$id] = $obj;
        return $obj;
    }
}
