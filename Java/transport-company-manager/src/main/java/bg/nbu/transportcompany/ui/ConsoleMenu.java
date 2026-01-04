package bg.nbu.transportcompany.ui;

import bg.nbu.transportcompany.dto.*;
import bg.nbu.transportcompany.entity.*;
import bg.nbu.transportcompany.exception.AppException;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.report.DriverRevenueDTO;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;
import bg.nbu.transportcompany.service.*;

import java.math.BigDecimal;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.List;

public class ConsoleMenu {

    private final CompanyService companyService;
    private final ClientService clientService;
    private final EmployeeService employeeService;
    private final VehicleService vehicleService;
    private final TransportService transportService;
    private final ReportService reportService;
    private final FileService fileService;

    private final InputReader input = new InputReader();

    public ConsoleMenu(CompanyService companyService,
                       ClientService clientService,
                       EmployeeService employeeService,
                       VehicleService vehicleService,
                       TransportService transportService,
                       ReportService reportService,
                       FileService fileService) {
        this.companyService = companyService;
        this.clientService = clientService;
        this.employeeService = employeeService;
        this.vehicleService = vehicleService;
        this.transportService = transportService;
        this.reportService = reportService;
        this.fileService = fileService;
    }

    public void run() {
        while (true) {
            try {
                printMainMenu();
                int choice = input.readInt("Choose: ");

                switch (choice) {
                    case 1 -> companiesMenu();
                    case 2 -> clientsMenu();
                    case 3 -> employeesMenu();
                    case 4 -> vehiclesMenu();
                    case 5 -> transportsMenu();
                    case 6 -> sortingFilteringMenu();
                    case 7 -> fileMenu();
                    case 8 -> reportsMenu();
                    case 0 -> { return; }
                    default -> System.out.println("Unknown option.");
                }
            } catch (AppException ex) {
                System.out.println("ERROR: " + ex.getMessage());
            } catch (Exception ex) {
                System.out.println("UNEXPECTED ERROR: " + ex.getMessage());
                ex.printStackTrace(System.out);
            }
        }
    }

    private void printMainMenu() {
        System.out.println();
        System.out.println("=====================================");
        System.out.println(" Transport Company Manager (Console)");
        System.out.println("=====================================");
        System.out.println("1) Companies (CRUD)");
        System.out.println("2) Clients (CRUD)");
        System.out.println("3) Employees (CRUD)");
        System.out.println("4) Vehicles (CRUD)");
        System.out.println("5) Transports (CRUD + Paid/Unpaid)");
        System.out.println("6) Sorting / Filtering");
        System.out.println("7) File I/O (Export/Import transports)");
        System.out.println("8) Reports");
        System.out.println("0) Exit");
    }

    private void companiesMenu() {
        System.out.println();
        System.out.println("== Companies ==");
        System.out.println("1) Create");
        System.out.println("2) List all");
        System.out.println("3) View by id");
        System.out.println("4) Update by id");
        System.out.println("5) Delete by id");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> {
                TransportCompanyDTO dto = new TransportCompanyDTO();
                dto.setName(input.readString("Name: "));
                dto.setAddress(input.readString("Address: "));
                TransportCompanyDTO created = companyService.create(dto);
                System.out.println("Created company id=" + created.getId());
            }
            case 2 -> ConsolePrinter.printList("Companies", companyService.getAll(),
                    c -> String.format("id=%d | name=%s | address=%s", c.getId(), c.getName(), c.getAddress()));
            case 3 -> {
                long id = input.readLong("Company id: ");
                TransportCompanyDTO c = companyService.getById(id);
                System.out.println(String.format("id=%d | name=%s | address=%s", c.getId(), c.getName(), c.getAddress()));
                System.out.println("Total PAID revenue: " + companyService.getTotalRevenue(id));
            }
            case 4 -> {
                long id = input.readLong("Company id: ");
                TransportCompanyDTO dto = new TransportCompanyDTO();
                dto.setName(input.readString("New name: "));
                dto.setAddress(input.readString("New address: "));
                TransportCompanyDTO updated = companyService.update(id, dto);
                System.out.println("Updated company id=" + updated.getId());
            }
            case 5 -> {
                long id = input.readLong("Company id: ");
                companyService.delete(id);
                System.out.println("Deleted.");
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void clientsMenu() {
        System.out.println();
        System.out.println("== Clients ==");
        System.out.println("1) Create");
        System.out.println("2) List all");
        System.out.println("3) View by id");
        System.out.println("4) Update by id");
        System.out.println("5) Delete by id");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> {
                ClientDTO dto = new ClientDTO();
                dto.setName(input.readString("Name: "));
                dto.setPhone(input.readString("Phone: "));
                dto.setEmail(input.readString("Email: "));
                ClientDTO created = clientService.create(dto);
                System.out.println("Created client id=" + created.getId());
            }
            case 2 -> ConsolePrinter.printList("Clients", clientService.getAll(),
                    c -> String.format("id=%d | name=%s | phone=%s | email=%s", c.getId(), c.getName(), c.getPhone(), c.getEmail()));
            case 3 -> {
                long id = input.readLong("Client id: ");
                ClientDTO c = clientService.getById(id);
                System.out.println(String.format("id=%d | name=%s | phone=%s | email=%s", c.getId(), c.getName(), c.getPhone(), c.getEmail()));
            }
            case 4 -> {
                long id = input.readLong("Client id: ");
                ClientDTO dto = new ClientDTO();
                dto.setName(input.readString("New name: "));
                dto.setPhone(input.readString("New phone: "));
                dto.setEmail(input.readString("New email: "));
                ClientDTO updated = clientService.update(id, dto);
                System.out.println("Updated client id=" + updated.getId());
            }
            case 5 -> {
                long id = input.readLong("Client id: ");
                clientService.delete(id);
                System.out.println("Deleted.");
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void employeesMenu() {
        System.out.println();
        System.out.println("== Employees ==");
        System.out.println("1) Create");
        System.out.println("2) List all");
        System.out.println("3) List by company id");
        System.out.println("4) View by id");
        System.out.println("5) Update by id");
        System.out.println("6) Delete by id");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> {
                EmployeeDTO dto = new EmployeeDTO();
                dto.setCompanyId(input.readLong("Company id: "));
                dto.setFirstName(input.readString("First name: "));
                dto.setLastName(input.readString("Last name: "));
                dto.setRole(input.readEnum("Role", EmployeeRole.class));
                if (dto.getRole() == EmployeeRole.DRIVER) {
                    dto.setQualification(input.readEnum("Qualification", DriverQualification.class));
                } else {
                    dto.setQualification(null);
                }
                dto.setSalary(input.readBigDecimal("Salary: "));
                EmployeeDTO created = employeeService.create(dto);
                System.out.println("Created employee id=" + created.getId());
            }
            case 2 -> ConsolePrinter.printList("Employees", employeeService.getAll(),
                    e -> String.format("id=%d | companyId=%d | %s %s | role=%s | qual=%s | salary=%s",
                            e.getId(), e.getCompanyId(), e.getFirstName(), e.getLastName(), e.getRole(), e.getQualification(), e.getSalary()));
            case 3 -> {
                long companyId = input.readLong("Company id: ");
                ConsolePrinter.printList("Employees", employeeService.getAllByCompany(companyId),
                        e -> String.format("id=%d | %s %s | role=%s | qual=%s | salary=%s",
                                e.getId(), e.getFirstName(), e.getLastName(), e.getRole(), e.getQualification(), e.getSalary()));
            }
            case 4 -> {
                long id = input.readLong("Employee id: ");
                EmployeeDTO e = employeeService.getById(id);
                System.out.println(String.format("id=%d | companyId=%d | %s %s | role=%s | qual=%s | salary=%s",
                        e.getId(), e.getCompanyId(), e.getFirstName(), e.getLastName(), e.getRole(), e.getQualification(), e.getSalary()));
            }
            case 5 -> {
                long id = input.readLong("Employee id: ");
                EmployeeDTO dto = new EmployeeDTO();
                dto.setCompanyId(1L);
                dto.setFirstName(input.readString("New first name: "));
                dto.setLastName(input.readString("New last name: "));
                dto.setRole(input.readEnum("New role", EmployeeRole.class));
                if (dto.getRole() == EmployeeRole.DRIVER) {
                    dto.setQualification(input.readEnum("New qualification", DriverQualification.class));
                } else {
                    dto.setQualification(null);
                }
                dto.setSalary(input.readBigDecimal("New salary: "));
                EmployeeDTO updated = employeeService.update(id, dto);
                System.out.println("Updated employee id=" + updated.getId());
            }
            case 6 -> {
                long id = input.readLong("Employee id: ");
                employeeService.delete(id);
                System.out.println("Deleted.");
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void vehiclesMenu() {
        System.out.println();
        System.out.println("== Vehicles ==");
        System.out.println("1) Create");
        System.out.println("2) List all");
        System.out.println("3) List by company id");
        System.out.println("4) View by id");
        System.out.println("5) Update by id");
        System.out.println("6) Delete by id");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> {
                VehicleDTO dto = new VehicleDTO();
                dto.setCompanyId(input.readLong("Company id: "));
                dto.setPlateNumber(input.readString("Plate number: "));
                dto.setType(input.readEnum("Vehicle type", VehicleType.class));
                dto.setSeatCapacity(input.readInt("Seat capacity: "));
                dto.setMaxLoadKg(Double.parseDouble(input.readString("Max load (kg): ")));
                VehicleDTO created = vehicleService.create(dto);
                System.out.println("Created vehicle id=" + created.getId());
            }
            case 2 -> ConsolePrinter.printList("Vehicles", vehicleService.getAll(),
                    v -> String.format("id=%d | companyId=%d | plate=%s | type=%s | seats=%d | maxLoadKg=%.1f",
                            v.getId(), v.getCompanyId(), v.getPlateNumber(), v.getType(), v.getSeatCapacity(), v.getMaxLoadKg()));
            case 3 -> {
                long companyId = input.readLong("Company id: ");
                ConsolePrinter.printList("Vehicles", vehicleService.getAllByCompany(companyId),
                        v -> String.format("id=%d | plate=%s | type=%s | seats=%d | maxLoadKg=%.1f",
                                v.getId(), v.getPlateNumber(), v.getType(), v.getSeatCapacity(), v.getMaxLoadKg()));
            }
            case 4 -> {
                long id = input.readLong("Vehicle id: ");
                VehicleDTO v = vehicleService.getById(id);
                System.out.println(String.format("id=%d | companyId=%d | plate=%s | type=%s | seats=%d | maxLoadKg=%.1f",
                        v.getId(), v.getCompanyId(), v.getPlateNumber(), v.getType(), v.getSeatCapacity(), v.getMaxLoadKg()));
            }
            case 5 -> {
                long id = input.readLong("Vehicle id: ");
                VehicleDTO dto = new VehicleDTO();
                dto.setCompanyId(1L);
                dto.setPlateNumber(input.readString("New plate: "));
                dto.setType(input.readEnum("New type", VehicleType.class));
                dto.setSeatCapacity(input.readInt("New seat capacity: "));
                dto.setMaxLoadKg(Double.parseDouble(input.readString("New max load (kg): ")));
                VehicleDTO updated = vehicleService.update(id, dto);
                System.out.println("Updated vehicle id=" + updated.getId());
            }
            case 6 -> {
                long id = input.readLong("Vehicle id: ");
                vehicleService.delete(id);
                System.out.println("Deleted.");
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void transportsMenu() {
        System.out.println();
        System.out.println("== Transports ==");
        System.out.println("1) Create");
        System.out.println("2) List all");
        System.out.println("3) View by id");
        System.out.println("4) Update by id");
        System.out.println("5) Delete by id");
        System.out.println("6) Mark PAID");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> {
                TransportDTO dto = new TransportDTO();
                dto.setCompanyId(input.readLong("Company id: "));
                dto.setClientId(input.readLong("Client id: "));
                dto.setDriverId(input.readLong("Driver (employee) id: "));
                dto.setVehicleId(input.readLong("Vehicle id: "));
                dto.setType(input.readEnum("Transport type", TransportType.class));
                dto.setOrigin(input.readString("Origin: "));
                dto.setDestination(input.readString("Destination: "));
                dto.setDepartureDate(input.readDate("Departure date"));
                dto.setArrivalDate(input.readDate("Arrival date"));

                if (dto.getType() == TransportType.PASSENGER) {
                    dto.setPassengerCount(input.readInt("Passenger count: "));
                    dto.setCargoWeightKg(null);
                } else {
                    dto.setCargoWeightKg(Double.parseDouble(input.readString("Cargo weight (kg): ")));
                    dto.setPassengerCount(null);
                }

                dto.setPrice(input.readBigDecimal("Price: "));
                dto.setPaymentStatus(input.readEnum("Payment status", PaymentStatus.class));

                TransportDTO created = transportService.create(dto);
                System.out.println("Created transport id=" + created.getId());
            }
            case 2 -> ConsolePrinter.printList("Transports", transportService.getAll(),
                    t -> String.format("id=%d | company=%d | client=%d | driver=%d | vehicle=%d | %s %s->%s | dep=%s | price=%s | %s",
                            t.getId(), t.getCompanyId(), t.getClientId(), t.getDriverId(), t.getVehicleId(),
                            t.getType(), t.getOrigin(), t.getDestination(), t.getDepartureDate(), t.getPrice(), t.getPaymentStatus()));
            case 3 -> {
                long id = input.readLong("Transport id: ");
                TransportDTO t = transportService.getById(id);
                System.out.println(String.format("id=%d | company=%d | client=%d | driver=%d | vehicle=%d",
                        t.getId(), t.getCompanyId(), t.getClientId(), t.getDriverId(), t.getVehicleId()));
                System.out.println(String.format("%s %s -> %s | dep=%s arr=%s | passengers=%s cargoKg=%s",
                        t.getType(), t.getOrigin(), t.getDestination(), t.getDepartureDate(), t.getArrivalDate(),
                        t.getPassengerCount(), t.getCargoWeightKg()));
                System.out.println("price=" + t.getPrice() + " | status=" + t.getPaymentStatus());
            }
            case 4 -> {
                long id = input.readLong("Transport id: ");
                TransportDTO existing = transportService.getById(id);

                TransportDTO dto = new TransportDTO();
                dto.setCompanyId(existing.getCompanyId());
                dto.setClientId(existing.getClientId());
                dto.setDriverId(existing.getDriverId());
                dto.setVehicleId(existing.getVehicleId());
                dto.setType(existing.getType());

                dto.setOrigin(input.readString("New origin: "));
                dto.setDestination(input.readString("New destination: "));
                dto.setDepartureDate(input.readDate("New departure date"));
                dto.setArrivalDate(input.readDate("New arrival date"));

                if (dto.getType() == TransportType.PASSENGER) {
                    dto.setPassengerCount(input.readInt("New passenger count: "));
                    dto.setCargoWeightKg(null);
                } else {
                    dto.setCargoWeightKg(Double.parseDouble(input.readString("New cargo weight (kg): ")));
                    dto.setPassengerCount(null);
                }

                dto.setPrice(input.readBigDecimal("New price: "));
                dto.setPaymentStatus(input.readEnum("New payment status", PaymentStatus.class));

                TransportDTO updated = transportService.update(id, dto);
                System.out.println("Updated transport id=" + updated.getId());
            }
            case 5 -> {
                long id = input.readLong("Transport id: ");
                transportService.delete(id);
                System.out.println("Deleted.");
            }
            case 6 -> {
                long id = input.readLong("Transport id: ");
                TransportDTO t = transportService.markPaid(id);
                System.out.println("Transport id=" + t.getId() + " marked as PAID.");
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void sortingFilteringMenu() {
        System.out.println();
        System.out.println("== Sorting / Filtering ==");
        System.out.println("1) List companies sorted by NAME (ASC)");
        System.out.println("2) List companies sorted by REVENUE (PAID, DESC)");
        System.out.println("3) List employees filtered by qualification + salary range");
        System.out.println("4) List transports filtered by destination (LIKE)");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> ConsolePrinter.printList("Companies (by name)", companyService.getAll(),
                    c -> String.format("id=%d | %s | %s", c.getId(), c.getName(), c.getAddress()));
            case 2 -> {
                List<CompanyRevenueDTO> list = companyService.getCompaniesSortedByRevenue();
                ConsolePrinter.printList("Companies (by revenue)", list,
                        r -> String.format("id=%d | %s | revenue=%s", r.getCompanyId(), r.getCompanyName(), r.getRevenue()));
            }
            case 3 -> {
                DriverQualification q = input.readEnum("Qualification", DriverQualification.class);
                BigDecimal min = input.readBigDecimal("Min salary: ");
                BigDecimal max = input.readBigDecimal("Max salary: ");
                List<EmployeeDTO> list = employeeService.findDriversByQualificationAndSalary(q, min, max);
                ConsolePrinter.printList("Drivers", list,
                        e -> String.format("id=%d | %s %s | qual=%s | salary=%s",
                                e.getId(), e.getFirstName(), e.getLastName(), e.getQualification(), e.getSalary()));
            }
            case 4 -> {
                String dest = input.readString("Destination contains: ");
                List<TransportDTO> list = transportService.findByDestination(dest);
                ConsolePrinter.printList("Transports", list,
                        t -> String.format("id=%d | %s -> %s | dep=%s | %s | %s",
                                t.getId(), t.getOrigin(), t.getDestination(), t.getDepartureDate(), t.getPrice(), t.getPaymentStatus()));
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void fileMenu() {
        System.out.println();
        System.out.println("== File I/O ==");
        System.out.println("1) Export transports to CSV");
        System.out.println("2) Preview CSV");
        System.out.println("3) Import transports from CSV");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> {
                Path out = Path.of("exports", "transports_export.csv");
                Path path = fileService.exportTransportsToCsv(out);
                System.out.println("Exported to: " + path.toAbsolutePath());
            }
            case 2 -> {
                String file = input.readString("CSV file path: ");
                List<TransportDTO> list = fileService.previewCsv(Path.of(file));
                ConsolePrinter.printList("CSV Preview", list,
                        t -> String.format("id=%s | company=%d | client=%d | driver=%d | vehicle=%d | %s %s->%s | %s | %s",
                                t.getId(), t.getCompanyId(), t.getClientId(), t.getDriverId(), t.getVehicleId(),
                                t.getType(), t.getOrigin(), t.getDestination(), t.getDepartureDate(), t.getPaymentStatus()));
            }
            case 3 -> {
                String file = input.readString("CSV file path: ");
                int imported = fileService.importTransportsFromCsv(Path.of(file));
                System.out.println("Imported transports: " + imported);
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }

    private void reportsMenu() {
        System.out.println();
        System.out.println("== Reports ==");
        System.out.println("1) Total number of transports");
        System.out.println("2) Total PAID revenue (sum)");
        System.out.println("3) Number of transports per driver");
        System.out.println("4) Company revenue for period (PAID)");
        System.out.println("5) Driver revenue for period (PAID)");
        System.out.println("0) Back");

        int choice = input.readInt("Choose: ");
        switch (choice) {
            case 1 -> System.out.println("Total transports: " + reportService.getTotalTransportsCount());
            case 2 -> System.out.println("Total PAID revenue: " + reportService.getTotalPaidRevenue());
            case 3 -> {
                List<DriverTransportCountDTO> list = reportService.getTransportsPerDriver();
                ConsolePrinter.printList("Transports per driver", list,
                        x -> String.format("driverId=%d | %s | count=%d", x.getDriverId(), x.getDriverName(), x.getTransportCount()));
            }
            case 4 -> {
                LocalDate start = input.readDate("Start date");
                LocalDate end = input.readDate("End date");
                List<CompanyRevenueDTO> list = reportService.getRevenuePerCompany(start, end);
                ConsolePrinter.printList("Revenue per company", list,
                        x -> String.format("companyId=%d | %s | revenue=%s", x.getCompanyId(), x.getCompanyName(), x.getRevenue()));
            }
            case 5 -> {
                LocalDate start = input.readDate("Start date");
                LocalDate end = input.readDate("End date");
                List<DriverRevenueDTO> list = reportService.getRevenuePerDriver(start, end);
                ConsolePrinter.printList("Revenue per driver", list,
                        x -> String.format("driverId=%d | %s | revenue=%s", x.getDriverId(), x.getDriverName(), x.getRevenue()));
            }
            case 0 -> { }
            default -> System.out.println("Unknown option.");
        }
    }
}
