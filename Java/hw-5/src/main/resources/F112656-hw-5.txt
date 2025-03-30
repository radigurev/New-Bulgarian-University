package org.example;

import java.io.*;
import java.math.*;
import java.util.*;
import java.util.stream.Stream;

import static java.util.stream.Collectors.toList;

enum ContractType {
    PERMANENT, PART_TIME, TRAINEE;

    private BigDecimal minSalary;

    public BigDecimal getMinSalary() {
        return minSalary;
    }

    public void setMinSalary(BigDecimal minSalary) {
        this.minSalary = minSalary;
    }
}

class Employee {
    private static int counter = 1;
    private int id;
    private String name;
    private int workedHours;
    private BigDecimal additionalSalary;
    private ContractType contractType;

    public Employee(String name, int workedHours, BigDecimal additionalSalary, ContractType contractType) {
        this.id = counter++;
        this.name = name;
        this.workedHours = workedHours;
        this.additionalSalary = additionalSalary;
        this.contractType = contractType;
    }

    public BigDecimal salary() {
        BigDecimal hourlyRate = contractType.getMinSalary().add(additionalSalary);

        return hourlyRate.multiply(BigDecimal.valueOf(workedHours));
    }

    public void increaseSalary(BigDecimal percentage) {
        if(percentage.compareTo(BigDecimal.valueOf(0)) <= 0) return;

        BigDecimal increase = additionalSalary
                .multiply(percentage)
                .divide(BigDecimal.valueOf(100), 2, RoundingMode.HALF_UP);

        additionalSalary = additionalSalary.add(increase);
    }

    public ContractType getContractType() {
        return contractType;
    }
}

class Company {
    private String name;
    private int maxEmployees;
    private List<Employee> employees;

    public Company(String name, int maxEmployees) {
        this.name = name;
        this.maxEmployees = maxEmployees;
        this.employees = new ArrayList<>();
    }

    public void hireEmployee(Employee e) {
        if (employees.contains(e)) return;

        if (employees.size() == maxEmployees) return;

        employees.add(e);
    }

    public void fireEmployee(Employee e) {
        employees.remove(e);
    }

    public BigDecimal averageSalary() {
        if (employees.isEmpty()) return BigDecimal.ZERO.setScale(0);

        BigDecimal sum = BigDecimal.ZERO;
        for (Employee e : employees) sum = sum.add(e.salary());

        return sum.divide(BigDecimal.valueOf(employees.size()), 2, RoundingMode.HALF_UP);
    }

    public void increaseSalaries(BigDecimal percentage) {
        for (Employee e : employees) e.increaseSalary(percentage);
    }

    private List<Employee> filterEmployeesByType(ContractType type) {
        List<Employee> filtered = new ArrayList<>();
        for (Employee e : employees) if (e.getContractType() == type) filtered.add(e);

        return filtered;
    }

    public BigDecimal averageSalaryByType(ContractType type) {
        List<Employee> filtered = filterEmployeesByType(type);

        if (filtered.isEmpty()) return BigDecimal.ZERO.setScale(0);

        BigDecimal sum = BigDecimal.ZERO;
        for (Employee e : filtered) sum = sum.add(e.salary());

        return sum.divide(BigDecimal.valueOf(filtered.size()), 2, RoundingMode.HALF_UP);
    }
}

public class Solution {
    public static void main(String[] args) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));

        String companyName = bufferedReader.readLine();

        int maxNumberOfEmployees = Integer.parseInt(bufferedReader.readLine().trim());

        List<Double> minSalaryList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(Double::parseDouble)
                .collect(toList());

        List<String> namesList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .collect(toList());

        List<Integer> workedHoursList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(Integer::parseInt)
                .collect(toList());

        List<String> contractTypeList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .collect(toList());

        List<Double> additionalSalaryList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(Double::parseDouble)
                .collect(toList());

        String contractType = bufferedReader.readLine();

        double increasePercentage = Double.parseDouble(bufferedReader.readLine().trim());

        ContractType.PERMANENT.setMinSalary(BigDecimal.valueOf(minSalaryList.get(0)));
        ContractType.PART_TIME.setMinSalary(BigDecimal.valueOf(minSalaryList.get(1)));
        ContractType.TRAINEE.setMinSalary(BigDecimal.valueOf(minSalaryList.get(2)));

        Company company = new Company(companyName, maxNumberOfEmployees);

        Employee employee1 = new Employee(namesList.get(0), workedHoursList.get(0), BigDecimal.valueOf(additionalSalaryList.get(0)), ContractType.valueOf(contractTypeList.get(0)));
        Employee employee2 = new Employee(namesList.get(1), workedHoursList.get(1), BigDecimal.valueOf(additionalSalaryList.get(1)), ContractType.valueOf(contractTypeList.get(1)));
        Employee employee3 = new Employee(namesList.get(2), workedHoursList.get(2), BigDecimal.valueOf(additionalSalaryList.get(2)), ContractType.valueOf(contractTypeList.get(2)));
        Employee employee4 = new Employee(namesList.get(3), workedHoursList.get(3), BigDecimal.valueOf(additionalSalaryList.get(3)), ContractType.valueOf(contractTypeList.get(3)));
        Employee employee5 = new Employee(namesList.get(4), workedHoursList.get(4), BigDecimal.valueOf(additionalSalaryList.get(4)), ContractType.valueOf(contractTypeList.get(4)));
        Employee employee6 = new Employee(namesList.get(5), workedHoursList.get(5), BigDecimal.valueOf(additionalSalaryList.get(5)), ContractType.valueOf(contractTypeList.get(5)));

        company.hireEmployee(employee1);
        company.hireEmployee(employee2);
        company.hireEmployee(employee3);
        company.hireEmployee(employee4);
        company.hireEmployee(employee5);
        company.hireEmployee(employee6);

        System.out.println(company.averageSalary());

        company.increaseSalaries(BigDecimal.valueOf(increasePercentage));

        System.out.println(company.averageSalary());

        System.out.println(company.averageSalaryByType(ContractType.valueOf(contractType)));

        bufferedReader.close();
    }
}
