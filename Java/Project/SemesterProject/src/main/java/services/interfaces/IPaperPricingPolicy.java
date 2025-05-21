package services.interfaces;

import models.enums.PaperSize;
import models.enums.PaperType;

import java.math.BigDecimal;

public interface IPaperPricingPolicy {
    BigDecimal priceFor(PaperType type, PaperSize size);
}
