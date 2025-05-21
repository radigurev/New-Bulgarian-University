package models;

import java.math.BigDecimal;

public class Ledger {
    private BigDecimal revenue = BigDecimal.ZERO;
    private BigDecimal paperCosts = BigDecimal.ZERO;
    private BigDecimal salaryCosts = BigDecimal.ZERO;

    public BigDecimal getRevenue() { return revenue; }
    public BigDecimal getPaperCosts() { return paperCosts; }
    public BigDecimal getSalaryCosts() { return salaryCosts; }
    public BigDecimal getNetProfit() { return revenue.subtract(paperCosts).subtract(salaryCosts); }

    public void addRevenue(BigDecimal amount) { revenue = revenue.add(amount); }
    public void addPaperCost(BigDecimal cost) { paperCosts = paperCosts.add(cost); }
    public void addSalaryCost(BigDecimal cost) { salaryCosts = salaryCosts.add(cost); }
}
