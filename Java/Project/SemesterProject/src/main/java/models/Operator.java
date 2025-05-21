package models;

import models.base.Employee;

import java.math.BigDecimal;

public class Operator extends Employee {
    private BigDecimal baseSalary;

    public Operator(String id, String name, BigDecimal baseSalary) {
        super(id, name);
        this.baseSalary = baseSalary;
    }

    @Override
    public BigDecimal getSalary(BigDecimal totalRevenue, BigDecimal managerBonusThreshold) {
        return baseSalary;
    }
}