package bg.nbu.transportcompany.validation;

import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.entity.Transport;
import bg.nbu.transportcompany.entity.TransportType;
import bg.nbu.transportcompany.entity.Vehicle;
import bg.nbu.transportcompany.entity.VehicleType;

import javax.validation.ConstraintValidator;
import javax.validation.ConstraintValidatorContext;

public class TransportAssignmentValidator implements ConstraintValidator<ValidTransportAssignment, Transport> {

    @Override
    public boolean isValid(Transport transport, ConstraintValidatorContext context) {
        if (transport == null) {
            return true;
        }

        Vehicle vehicle = transport.getVehicle();
        if (vehicle == null || vehicle.getType() == null) {
            return true;
        }

        boolean ok = true;
        context.disableDefaultConstraintViolation();

        TransportType type = transport.getType();
        if (type == null) {
            return true;
        }

        if (type == TransportType.PASSENGER) {
            Integer passengers = transport.getPassengerCount();
            if (passengers == null || passengers <= 0) {
                context.buildConstraintViolationWithTemplate("passengerCount must be positive for PASSENGER transport")
                        .addPropertyNode("passengerCount").addConstraintViolation();
                ok = false;
            } else if (passengers > vehicle.getSeatCapacity()) {
                context.buildConstraintViolationWithTemplate("passengerCount exceeds vehicle seat capacity")
                        .addPropertyNode("passengerCount").addConstraintViolation();
                ok = false;
            }
        }

        if (type == TransportType.CARGO) {
            Double cargoKg = transport.getCargoWeightKg();
            if (cargoKg == null || cargoKg <= 0) {
                context.buildConstraintViolationWithTemplate("cargoWeightKg must be positive for CARGO transport")
                        .addPropertyNode("cargoWeightKg").addConstraintViolation();
                ok = false;
            } else if (cargoKg > vehicle.getMaxLoadKg()) {
                context.buildConstraintViolationWithTemplate("cargoWeightKg exceeds vehicle max load")
                        .addPropertyNode("cargoWeightKg").addConstraintViolation();
                ok = false;
            }
        }

        if (transport.getDriver() != null) {
            DriverQualification q = transport.getDriver().getQualification();
            VehicleType vType = vehicle.getType();
            if (q == null) {
                context.buildConstraintViolationWithTemplate("driver qualification is required")
                        .addPropertyNode("driver").addConstraintViolation();
                ok = false;
            } else if (!q.supports(vType)) {
                context.buildConstraintViolationWithTemplate("driver qualification does not match vehicle type")
                        .addPropertyNode("driver").addConstraintViolation();
                ok = false;
            }
        }

        return ok;
    }
}
