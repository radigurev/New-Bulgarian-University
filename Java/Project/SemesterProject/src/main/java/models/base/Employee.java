package models.base;

import java.io.Serializable;
import java.math.BigDecimal;

public abstract class Employee implements Serializable {
    private final String name;
    protected final BigDecimal baseSalary;

    protected Employee(String name, BigDecimal baseSalary) {
        this.name = name;
        this.baseSalary = baseSalary;
    }

    public String getName() {

        return name;
    }

    public abstract BigDecimal getSalary(BigDecimal revenue, BigDecimal threshold);
}
