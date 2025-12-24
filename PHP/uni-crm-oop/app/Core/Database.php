<?php
declare(strict_types=1);

namespace App\Core;

use PDO;

final class Database
{
    private PDO $pdo;

    public function __construct(array $config)
    {
        $db = $config['db'];
        $dsn = sprintf(
            'mysql:host=%s;dbname=%s;charset=%s',
            $db['host'],
            $db['name'],
            $db['charset']
        );

        $this->pdo = new PDO($dsn, $db['user'], $db['pass'], [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        ]);
    }

    public function pdo(): PDO { return $this->pdo; }

    public function scalar(string $sql, array $params = []): mixed
    {
        $st = $this->pdo->prepare($sql);
        $st->execute($params);
        $row = $st->fetch();
        if (!$row) return null;
        return array_values($row)[0];
    }
}
