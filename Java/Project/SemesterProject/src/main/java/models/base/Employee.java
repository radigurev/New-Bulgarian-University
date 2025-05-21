package models.base;

import java.io.Serializable;
import java.math.BigDecimal;

public abstract class Employee implements Serializable {
    private String id;
    private String name;

    public Employee(String id, String name) {
        this.id = id;
        this.name = name;
    }

    public String getId() { return id; }
    public String getName() { return name; }
    public abstract BigDecimal getSalary(BigDecimal totalRevenue, BigDecimal managerBonusThreshold);
}
