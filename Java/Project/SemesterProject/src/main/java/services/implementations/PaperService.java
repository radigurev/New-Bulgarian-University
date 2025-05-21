package services.implementations;

import models.enums.PaperSize;
import models.enums.PaperType;
import services.interfaces.IPaperService;

import java.math.BigDecimal;
import java.math.MathContext;
import java.util.EnumMap;
import java.util.Map;

public class PaperService implements IPaperService {
    private final Map<PaperType, BigDecimal> basePriceMap = new EnumMap<>(PaperType.class);
    private final BigDecimal step;

    public PaperService(Map<PaperType, BigDecimal> basePriceForA5, BigDecimal step) {
        this.basePriceMap.putAll(basePriceForA5);
        this.step = step;
    }

    @Override
    public BigDecimal basePrice(PaperType type) {
        return basePriceMap.get(type);
    }

    @Override
    public BigDecimal priceFor(PaperType type, PaperSize size) {
        BigDecimal base = basePrice(type);
        return base.multiply(step.add(BigDecimal.ONE).pow(size.stepsUp(), MathContext.DECIMAL64));
    }
}
