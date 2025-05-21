package models;
import models.base.Employee;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;

public class PrintingHouse {
    private String name;
    private int discountThreshold;
    private BigDecimal discountRate;
    private BigDecimal managerBonusThreshold;
    private Ledger ledger;
    private List<PrintingMachine> machines;
    private List<Employee> staff;

    public PrintingHouse(String name) {
        this.name = name;
        this.discountThreshold = 1000;
        this.discountRate = BigDecimal.valueOf(0.05);
        this.managerBonusThreshold = BigDecimal.valueOf(10000);
        this.ledger = new Ledger();
        this.machines = new ArrayList<>();
        this.staff = new ArrayList<>();
    }

    public String getName() { return name; }
    public int getDiscountThreshold() { return discountThreshold; }
    public void setDiscountThreshold(int discountThreshold) { this.discountThreshold = discountThreshold; }

    public BigDecimal getDiscountRate() { return discountRate; }
    public void setDiscountRate(BigDecimal discountRate) { this.discountRate = discountRate; }

    public BigDecimal getManagerBonusThreshold() { return managerBonusThreshold; }
    public void setManagerBonusThreshold(BigDecimal managerBonusThreshold) { this.managerBonusThreshold = managerBonusThreshold; }

    public Ledger getLedger() { return ledger; }
    public List<PrintingMachine> getMachines() { return machines; }
    public List<Employee> getStaff() { return staff; }

    public void addMachine(PrintingMachine machine) { machines.add(machine); }
    public void addEmployee(Employee e) { staff.add(e); }
}