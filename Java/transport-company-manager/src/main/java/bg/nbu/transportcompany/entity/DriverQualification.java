package bg.nbu.transportcompany.entity;

public enum DriverQualification {
    PASSENGER_TRANSPORT,
    CARGO_TRANSPORT,
    HAZARDOUS_MATERIALS;

    public boolean supports(VehicleType vehicleType) {
        if (vehicleType == null) {
            return false;
        }
        return switch (vehicleType) {
            case BUS -> this == PASSENGER_TRANSPORT;
            case VAN -> this == PASSENGER_TRANSPORT || this == CARGO_TRANSPORT;
            case TRUCK -> this == CARGO_TRANSPORT;
            case TANKER -> this == HAZARDOUS_MATERIALS;
        };
    }
}
