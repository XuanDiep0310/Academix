import type { NextAuthOptions } from "next-auth";
import GoogleProvider from "next-auth/providers/google";
import CredentialsProvider from "next-auth/providers/credentials";

export const authOptions: NextAuthOptions = {
  // Configure one or more authentication providers
  secret: process.env.NO_SECRET,
  providers: [
    CredentialsProvider({
      // The name to display on the sign in form (e.g. "Sign in with...")
      name: "Academix",
      // `credentials` is used to generate a form on the sign in page.
      // You can specify which fields should be submitted, by adding keys to the `credentials` object.
      // e.g. domain, username, password, 2FA token, etc.
      // You can pass any HTML attribute to the <input> tag through the object.
      credentials: {
        username: { label: "Username", type: "text", placeholder: "jsmith" },
        password: { label: "Password", type: "password" },
      },
      async authorize(credentials, req) {
        // const res = await sendRequest<JWT>({
        //   url: `${process.env.NEXT_PUBLIC_API_URL}/api/Auth/login`,
        //   method: "POST",
        //   body: {
        //     email: credentials?.username,
        //       password: credentials?.password,
        //   },
        // });
        // const res = await fetch(
        //   `${process.env.NEXT_PUBLIC_API_URL}/api/Auth/login`,
        //   {
        //     method: "POST",
        //     headers: {
        //       "Content-Type": "application/json",
        //     },
        //     body: JSON.stringify({
        //       email: "admin",
        //       password: "123456",
        //     }),
        //   }
        // );
        // Add logic here to look up the user from the credentials supplied
        const user = { id: "1", name: "J Smith", email: "jsmith@example.com" };

        if (user) {
          // Any object returned will be saved in `user` property of the JWT
          return user;
        } else {
          // If you return null then an error will be displayed advising the user to check their details.
          return null;

          // You can also Reject this callback with an Error thus the user will be sent to the error page with the error message as a query parameter
        }
      },
    }),
    GoogleProvider({
      clientId: process.env.GOOGLE_CLIENT_ID!,
      clientSecret: process.env.GOOGLE_CLIENT_SECRET!,
      authorization: {
        params: {
          prompt: "consent",
          access_type: "offline",
          response_type: "code",
        },
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user, account, profile, trigger }) {
      if (trigger === "signIn" && account?.provider !== "credentials") {
        // const res = await sendRequest<JWT>({
        //   url: "/api/..",
        //   method: "POST",
        //   body: {
        //     type: account?.provider?.toLocaleUpperCase(),
        //     username: user.email,
        //   },
        // });
        // console.log(">> check data", res);
        // if(res.data) {
        //   token.token = res.data?.token;
        //   token.refreshToken= res.data?.refreshToken;
        //   token.user = res.data?.user;
        // }
      }
      // if (trigger === "signIn" && account?.provider === "credentials"){

      //   // token.token = user.token;
      //   //   token.refreshToken=  user.refreshToken;
      //   //   token.user =  user.user;
      // }
      return token;
    },
    async session({ session, user, token }) {
      // session.token = token.token;
      // session.refreshToken = token.refreshToken;
      // session.user = token.user;
      return session;
    },
  },
};
