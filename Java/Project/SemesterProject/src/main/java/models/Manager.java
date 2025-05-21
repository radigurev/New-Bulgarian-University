package models;

import models.base.Employee;

import java.math.BigDecimal;

public class Manager extends Employee {
    private BigDecimal baseSalary;
    private BigDecimal bonus;

    public Manager(String id, String name, BigDecimal baseSalary, BigDecimal bonus) {
        super(id, name);
        this.baseSalary = baseSalary;
        this.bonus = bonus;
    }

    @Override
    public BigDecimal getSalary(BigDecimal totalRevenue, BigDecimal managerBonusThreshold) {
        BigDecimal salary = baseSalary;
        if (totalRevenue.compareTo(managerBonusThreshold) > 0) {
            salary = salary.add(bonus);
        }
        return salary;
    }
}
