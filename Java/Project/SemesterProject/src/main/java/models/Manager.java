package models;

import models.base.Employee;

import java.math.BigDecimal;

public class Manager extends Employee {

    private final BigDecimal bonusRate;

    public Manager(String name, BigDecimal baseSalary, BigDecimal bonusRate) {
        super(name, baseSalary);
        this.bonusRate = bonusRate;
    }

    @Override
    public BigDecimal getSalary(BigDecimal revenue, BigDecimal threshold) {
        if (revenue.compareTo(threshold) > 0) {
            return baseSalary.add(baseSalary.multiply(bonusRate));
        }
        return baseSalary;
    }
}
