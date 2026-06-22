package com.inf.olympics.config;

import lombok.extern.slf4j.Slf4j;
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.springframework.stereotype.Component;

@Aspect
@Component
@Slf4j
public class LoggingAspect {

    @Around("execution(* com.inf.olympics.application.service..*ServiceImpl.*(..)) " +
            "|| execution(* com.inf.olympics.application.service.*Service.*(..)) " +
            "|| execution(* com.inf.olympics.domain.ranking.*Strategy.*(..))")
    public Object logServiceCalls(ProceedingJoinPoint joinPoint) throws Throwable {
        String signature = joinPoint.getSignature().toShortString();
        long start = System.nanoTime();
        try {
            Object result = joinPoint.proceed();
            if (log.isDebugEnabled()) {
                long durationMicros = (System.nanoTime() - start) / 1_000;
                log.debug("{} completed in {} us", signature, durationMicros);
            }
            return result;
        } catch (Throwable ex) {
            log.warn("{} threw {}: {}", signature, ex.getClass().getSimpleName(), ex.getMessage());
            throw ex;
        }
    }
}
