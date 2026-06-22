package com.inf.olympics.infrastructure.security;

import com.inf.olympics.infrastructure.persistence.entity.UserAccountEntity;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jws;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import javax.crypto.SecretKey;
import java.time.Instant;
import java.util.Date;
import java.util.List;

@Service
@RequiredArgsConstructor
public class JwtService {

    private final JwtProperties properties;

    public IssuedToken issue(UserAccountEntity user) {
        Instant now = Instant.now();
        Instant exp = now.plusSeconds(properties.ttlMinutes() * 60);
        List<String> authorities = user.getRoles().stream().map(Enum::name).toList();
        String token = Jwts.builder()
                .issuer(properties.issuer())
                .subject(user.getUsername())
                .issuedAt(Date.from(now))
                .expiration(Date.from(exp))
                .claim("roles", authorities)
                .claim("uid", user.getId())
                .signWith(signingKey())
                .compact();
        return new IssuedToken(token, exp, user.getUsername(), authorities);
    }

    public ParsedToken parse(String token) {
        Jws<Claims> jws = Jwts.parser()
                .verifyWith(signingKey())
                .requireIssuer(properties.issuer())
                .build()
                .parseSignedClaims(token);

        Claims claims = jws.getPayload();
        @SuppressWarnings("unchecked")
        List<String> roles = (List<String>) claims.get("roles");
        return new ParsedToken(claims.getSubject(), roles == null ? List.of() : roles);
    }

    private SecretKey signingKey() {
        byte[] keyBytes = properties.secret().getBytes(java.nio.charset.StandardCharsets.UTF_8);
        if (keyBytes.length < 32) {
            keyBytes = padTo32(keyBytes);
        }
        return Keys.hmacShaKeyFor(keyBytes);
    }

    private static byte[] padTo32(byte[] in) {
        byte[] out = new byte[32];
        System.arraycopy(in, 0, out, 0, Math.min(32, in.length));
        return out;
    }

    public record IssuedToken(String value, Instant expiresAt, String username, List<String> roles) {
    }

    public record ParsedToken(String username, List<String> roles) {
    }
}
