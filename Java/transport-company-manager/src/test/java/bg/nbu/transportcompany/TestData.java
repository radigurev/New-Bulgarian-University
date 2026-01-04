package bg.nbu.transportcompany;

import bg.nbu.transportcompany.dto.*;
import bg.nbu.transportcompany.entity.*;

import java.math.BigDecimal;
import java.time.LocalDate;

public final class TestData {

    private TestData() {
    }

    public static TransportCompanyDTO company(String name) {
        TransportCompanyDTO dto = new TransportCompanyDTO();
        dto.setName(name);
        dto.setAddress("Sofia");
        return dto;
    }

    public static ClientDTO client(String name) {
        ClientDTO dto = new ClientDTO();
        dto.setName(name);
        dto.setPhone("000");
        dto.setEmail("a@b.com");
        return dto;
    }

    public static EmployeeDTO driver(Long companyId, String first, DriverQualification q, BigDecimal salary) {
        EmployeeDTO dto = new EmployeeDTO();
        dto.setCompanyId(companyId);
        dto.setFirstName(first);
        dto.setLastName("D");
        dto.setRole(EmployeeRole.DRIVER);
        dto.setQualification(q);
        dto.setSalary(salary);
        return dto;
    }

    public static VehicleDTO bus(Long companyId, String plate, int seats) {
        VehicleDTO dto = new VehicleDTO();
        dto.setCompanyId(companyId);
        dto.setPlateNumber(plate);
        dto.setType(VehicleType.BUS);
        dto.setSeatCapacity(seats);
        dto.setMaxLoadKg(1000);
        return dto;
    }

    public static VehicleDTO truck(Long companyId, String plate, double maxLoadKg) {
        VehicleDTO dto = new VehicleDTO();
        dto.setCompanyId(companyId);
        dto.setPlateNumber(plate);
        dto.setType(VehicleType.TRUCK);
        dto.setSeatCapacity(2);
        dto.setMaxLoadKg(maxLoadKg);
        return dto;
    }

    public static TransportDTO passengerTransport(Long companyId, Long clientId, Long driverId, Long vehicleId, int passengers, BigDecimal price) {
        TransportDTO dto = new TransportDTO();
        dto.setCompanyId(companyId);
        dto.setClientId(clientId);
        dto.setDriverId(driverId);
        dto.setVehicleId(vehicleId);
        dto.setType(TransportType.PASSENGER);
        dto.setOrigin("Sofia");
        dto.setDestination("Plovdiv");
        dto.setDepartureDate(LocalDate.now().minusDays(2));
        dto.setArrivalDate(LocalDate.now().minusDays(1));
        dto.setPassengerCount(passengers);
        dto.setCargoWeightKg(null);
        dto.setPrice(price);
        dto.setPaymentStatus(PaymentStatus.UNPAID);
        return dto;
    }

    public static TransportDTO cargoTransport(Long companyId, Long clientId, Long driverId, Long vehicleId, double kg, BigDecimal price) {
        TransportDTO dto = new TransportDTO();
        dto.setCompanyId(companyId);
        dto.setClientId(clientId);
        dto.setDriverId(driverId);
        dto.setVehicleId(vehicleId);
        dto.setType(TransportType.CARGO);
        dto.setOrigin("Varna");
        dto.setDestination("Burgas");
        dto.setDepartureDate(LocalDate.now().minusDays(5));
        dto.setArrivalDate(LocalDate.now().minusDays(4));
        dto.setPassengerCount(null);
        dto.setCargoWeightKg(kg);
        dto.setPrice(price);
        dto.setPaymentStatus(PaymentStatus.PAID);
        return dto;
    }
}
