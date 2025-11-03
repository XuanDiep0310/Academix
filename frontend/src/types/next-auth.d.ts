import NextAuth, { DefaultSession } from "next-auth";
import { JWT } from "next-auth/jwt";

interface IUser {
  userId: string;
  email: string;
  displayName: string;
  avatarUrl: string;
  organizationId: 0;
  roles: [string];
  permissions: [string];
}
declare module "next-auth/jwt" {
  /** Returned by the `jwt` callback and `getToken`, when using JWT sessions */
  interface JWT {
    token: string;
    refreshToken: string;
    expiresAt: string;
    user: IUser;
  }
}

declare module "next-auth" {
  /**
   * Returned by `useSession`, `getSession` and received as a prop on the `SessionProvider` React Context
   */
  interface Session {
    token: string;
    refreshToken: string;
    expiresAt: string;
    user: IUser;
  }
}
