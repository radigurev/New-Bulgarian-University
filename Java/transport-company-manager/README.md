# Transport Company Management – Console Application (Java + Hibernate + MySQL)

Console CRUD system for:
- Transport companies
- Clients
- Employees (incl. drivers with qualifications)
- Vehicles
- Transports (Passenger/Cargo) + paid/unpaid status

Also includes:
- Sorting & filtering (companies by revenue, drivers by qualification+salary, transports by destination)
- Reports (total transports, total paid revenue, transports per driver, revenue per company/driver for period)
- Export/Import transports to CSV
- Hibernate Validator + custom validation annotation
- Integration tests + JaCoCo coverage check (80%)

---

## Requirements
- Java 17+
- Maven 3.9+
- MySQL 8+

---

## 1) MySQL setup

Create a MySQL user (or use `root`) and allow local connections.

Default config (change if needed):
- DB: `transport_company`
- Test DB: `transport_company_test`
- User: `root`
- Pass: `root`

If your MySQL user/password are different, change:
- `src/main/resources/application.properties`
- `src/main/resources/application-test.properties`

Or run with system properties, for example:
```bash
mvn -Ddb.user=root -Ddb.pass=YOURPASS -Ddb.url="jdbc:mysql://localhost:3306/transport_company?createDatabaseIfNotExist=true&useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=UTC" test
```

---

## 2) Run the console application

```bash
mvn clean package
mvn exec:java
```

The app starts a menu. You can create companies, clients, drivers, vehicles, and then create transports.

---

## 3) Run tests + coverage

```bash
mvn clean test
```

JaCoCo is configured with **minimum 80% line coverage** (UI menu is excluded from coverage check).

Tests use TRUNCATE to clean the database (see `DbTestUtil`).

---

## Notes
- Relationships use `FetchType.LAZY`.
- Company -> Employees/Vehicles uses `CascadeType.PERSIST` (as requested).
- `Transport` is validated with a **custom annotation** `@ValidTransportAssignment`:
  - Passenger transports require passengerCount and vehicle seats to be enough.
  - Cargo transports require cargoWeightKg and vehicle max load to be enough.
  - Driver must have a matching qualification.
  - Driver and vehicle must belong to the same company as the transport.

Export file is saved in `exports/transports_export.csv` by default.
