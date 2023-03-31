package com.microsoft.semantickernel.skills.random;

import com.microsoft.azure.functions.ExecutionContext;
import com.microsoft.azure.functions.HttpRequestMessage;
import com.microsoft.azure.functions.HttpResponseMessage;
import com.microsoft.azure.functions.HttpStatus;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.Spy;

import java.io.IOException;
import java.io.InputStream;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.logging.Logger;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class RandomActivitySkillTest {

    @Mock
    HttpResponse<InputStream> mockResponse;

    @Spy
    HttpClient httpClient;
    @Test
    @DisplayName("should return a random activity when the API call is successful")
    void getRandomActivityWhenApiCallIsSuccessful() throws IOException, InterruptedException {
        var request = mock(HttpRequestMessage.class, RETURNS_DEEP_STUBS);
        ExecutionContext context = mock(ExecutionContext.class, RETURNS_DEEP_STUBS);
        when(httpClient.send(any(), any(HttpResponse.BodyHandlers.ofInputStream().getClass())))
                .thenReturn(mockResponse);

        HttpResponseMessage actual = new RandomActivitySkill().getRandomActivity(request, context);

        assertEquals(HttpStatus.OK, actual.getStatus());
        assertEquals("Test Activity", actual.getBody());
    }
}