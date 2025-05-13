package models;

import models.enums.PaperSize;

import java.math.BigDecimal;

public class Publication {
    private final String title;
    private final int totalCopies;
    private final PaperSize pageSize;
    private final int pagesPerCopy;
    private final PrintMode mode;
    private final BigDecimal pricePerCopy;

    public Publication(String title,
                       int totalCopies,
                       PaperSize pageSize,
                       int pagesPerCopy,
                       PrintMode mode,
                       BigDecimal pricePerCopy) {
        this.title = title;
        this.totalCopies = totalCopies;
        this.pageSize = pageSize;
        this.pagesPerCopy = pagesPerCopy;
        this.mode = mode;
        this.pricePerCopy = pricePerCopy;
    }

    public String getTitle() { return title; }
    public int getTotalCopies() { return totalCopies; }
    public PaperSize getPageSize() { return pageSize; }
    public int getPagesPerCopy() { return pagesPerCopy; }
    public PrintMode getMode() { return mode; }
    public BigDecimal getPricePerCopy() { return pricePerCopy; }

    public BigDecimal totalRevenue(BigDecimal discountRate, int discountThreshold) {
        BigDecimal effectivePrice = pricePerCopy;
        if (totalCopies > discountThreshold) {
            effectivePrice = pricePerCopy.multiply(BigDecimal.ONE.subtract(discountRate));
        }
        return effectivePrice.multiply(BigDecimal.valueOf(totalCopies));
    }
}
