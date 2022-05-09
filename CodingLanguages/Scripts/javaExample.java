import java.io.*;
import javax.servlet.*;
import javax.servlet.http.*;

// Extending HttpServlet class
public class HelloServlet extends HttpServlet {
 
   private String msg;

   public void init() throws ServletException {
      // Do required initialization
      msg = "Hello Servlet";
   }

   public void doGet(HttpServletRequest request, HttpServletResponse response)
      throws ServletException, IOException {
      
      // Setting content type for response
      response.setContentType("text/html");

      // printing msg to browser in h1 heading
      PrintWriter out = response.getWriter();
      out.println("<h1>" + msg + "</>");
   }

   public void destroy() {
      // code to execute while destroy servlet.
   }
}