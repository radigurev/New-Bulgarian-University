package models;

import models.base.Employee;

import java.math.BigDecimal;

public class Operator extends Employee {

    public Operator(String name, BigDecimal baseSalary) {
        super(name, baseSalary);
    }

    @Override
    public BigDecimal getSalary(BigDecimal revenue, BigDecimal threshold) {
        return baseSalary;
    }
}
