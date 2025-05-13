package models;

import models.enums.*;

import java.math.BigDecimal;

public class Paper {
    private final PaperType type;
    private final PaperSize size;
    private final BigDecimal pricePerSheet;

    public Paper(PaperType type, PaperSize size, BigDecimal pricePerSheet) {
        this.type = type;
        this.size = size;
        this.pricePerSheet = pricePerSheet;
    }

    public PaperType getType() {
        return type;
    }

    public PaperSize getSize() {
        return size;
    }

    public BigDecimal getPricePerSheet() {
        return pricePerSheet;
    }
}