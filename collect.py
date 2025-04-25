import os

def collect_files(root_dir, output_file):
    """
    Traverses through directories, collects shader, HLSL, C#, Compute Shader,
    and CG include files, and writes their content and relative paths
    to a single output file.

    Args:
        root_dir (str): The root directory to start the search from.
        output_file (str): The path to the output text file.
    """
    try:
        # Open the output file in write mode with UTF-8 encoding
        with open(output_file, 'w', encoding='utf-8') as outfile:
            print(f"Scanning directory: '{os.path.abspath(root_dir)}'...")
            file_count = 0
            # Walk through the directory tree starting from root_dir
            for dirpath, dirnames, filenames in os.walk(root_dir):
                for filename in filenames:
                    # Check if the file has one of the desired extensions
                    if filename.endswith(('.shader', '.cginc', '.hlsl', '.cs', '.compute', '.json')):
                        # Construct the full path to the file
                        filepath = os.path.join(dirpath, filename)
                        try:
                            # Calculate the path relative to the root_dir
                            # This is the path relative to the 'Assets' folder if root_dir is './Assets'
                            relative_path = os.path.relpath(filepath, root_dir)

                            # Open the found file in read mode
                            with open(filepath, 'r', encoding='utf-8', errors='ignore') as infile:
                                content = infile.read()

                            # Write a header indicating the file's relative path
                            outfile.write(f"// File: {relative_path.replace(os.sep, '/')}\n") # Use forward slashes for consistency
                            outfile.write(f"// Full Path: {os.path.abspath(filepath)}\n") # Optionally include full path
                            outfile.write("-" * 40 + "\n") # Separator line

                            # Write the content of the file
                            outfile.write(content)
                            outfile.write("\n\n" + "=" * 40 + "\n\n") # Add separators between files
                            file_count += 1

                        except (IOError, OSError) as e:
                            print(f"Error reading file: {filepath} - {e}")
                            # Optionally log this error more formally
                            continue # Skip to the next file
                        except UnicodeDecodeError as e:
                            print(f"Encoding error in file: {filepath} - {e}. Skipping content.")
                            # Write header even if content can't be read
                            outfile.write(f"// File: {relative_path.replace(os.sep, '/')}\n")
                            outfile.write(f"// Full Path: {os.path.abspath(filepath)}\n")
                            outfile.write("-" * 40 + "\n")
                            outfile.write(f"// Error: Could not decode file content ({e})\n")
                            outfile.write("\n\n" + "=" * 40 + "\n\n")
                            continue # Skip to the next file
            print(f"Found and processed {file_count} files.")

    except IOError as e:
        print(f"Error opening or writing to output file: {output_file} - {e}")
        # Consider more robust error handling, like retrying or alerting the user.
    except Exception as e:
        print(f"An unexpected error occurred: {e}")

if __name__ == "__main__":
    # Get the root directory from the user. Use a default if none provided.
    root_dir_input = input("Enter the root directory to scan (default: './Assets'): ").strip()
    root_dir = root_dir_input if root_dir_input else "./Assets" # Default to Assets

    # Check if the specified root directory exists
    if not os.path.isdir(root_dir):
        print(f"Error: The directory '{root_dir}' does not exist or is not a directory.")
        exit(1) # Use a non-zero exit code for errors

    # Define the output file name
    output_file = "collected_code.txt"
    print(f"Collecting files from '{os.path.abspath(root_dir)}' and writing to '{output_file}'...")

    # Call the function to collect files
    collect_files(root_dir, output_file)

    print(f"File collection complete. Output saved to '{output_file}'.")
