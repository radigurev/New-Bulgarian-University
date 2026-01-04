package bg.nbu.transportcompany.validation;

import javax.validation.Constraint;
import javax.validation.Payload;
import java.lang.annotation.*;

@Documented
@Constraint(validatedBy = TransportAssignmentValidator.class)
@Target({ElementType.TYPE})
@Retention(RetentionPolicy.RUNTIME)
public @interface ValidTransportAssignment {
    String message() default "Invalid transport assignment (type/capacity/qualification mismatch)";
    Class<?>[] groups() default {};
    Class<? extends Payload>[] payload() default {};
}
