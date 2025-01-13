import semver
import yaml
import argparse

def main():
    args = get_args()
    yaml_data = {}
    with open(args.file) as stream:
        yaml_data = yaml.safe_load(stream)
    
    if yaml_data['version'] is None:
        raise Exception('Version not found in file')

    version = semver.Version.parse(yaml_data['version'])
    version = version.bump_patch()
    yaml_data['version'] = "{}".format(version)

    if args.app_version != None:
        yaml_data['appVersion'] = args.app_version

    with open(args.file, 'w', encoding='utf8') as file:
        yaml.dump(yaml_data, file)

def get_args():
    parser = argparse.ArgumentParser()

    parser.add_argument('--file', required=True, type=str, help='The relative path to the file to be parse. Required.')
    parser.add_argument('--app-version', type=str, help='Option arg. If set, will update the appVersion property of the Helm chart')
    return parser.parse_args()

if __name__ == '__main__':
    main()