import xml.etree.ElementTree as ET
import semver
import argparse


def main():
    args = get_args()

    root = ET.parse(args.file)
    version_element = root.find('PropertyGroup/Version')
    ver = semver.Version.parse(version_element.text)
    update_version = get_update_version(ver, args.build_type, args.custom_version)

    update_version_string = "{}".format(update_version)
    version_element.text = update_version_string
    root.write(args.file)
    print(update_version_string)

def get_args():
    parser = argparse.ArgumentParser()

    parser.add_argument('--file', required=True, type=str, help='The relative path to the file to be parse. Required.')
    parser.add_argument('--build-type', required=True, type=str, help='The build type, can be MAJOR, MINOR, PATCH, or CUSTOM.')
    parser.add_argument('--custom-version', type=str, help='If --build-type was CUSTOM, specify the customer version here.', default='')
    return parser.parse_args()

def get_update_version(current_version, build_type, custom_version):
    update_version = current_version
    if build_type == 'MAJOR':
        update_version = update_version.bump_major()
    elif build_type == 'MINOR':
        update_version = update_version.bump_minor()
    elif build_type == 'PATCH':
        update_version = update_version.bump_patch()
    elif build_type == 'CUSTOM':
        validate_custom_version(custom_version)
        update_version = semver.Version.parse(custom_version)
    else:
        raise Exception("Invalid build type")

    return "{}".format(update_version)

def validate_custom_version(custom_version):
    if custom_version == '':
        raise Exception("No custom version was provided. Please provide one using the --custom-version arg.")


if __name__ == '__main__':
    main()