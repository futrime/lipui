interface ToothItemResult {
  packages: {
    tooth: string;
    version: string;
    information: {
      author: string;
      description: string;
      homepage: string;
      license: string;
      name: string;
    };
  }[];
  message: string;
}
export { type ToothItemResult };
