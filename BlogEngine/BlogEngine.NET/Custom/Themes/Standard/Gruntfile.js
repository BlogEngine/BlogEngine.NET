module.exports = function (grunt) {
    grunt.initConfig({
        sass: {
            options: {
                noCache: true
            },
            dist: {
                options: {
                    style: 'compressed',
                },
                files: {
                    'src/css/styles.min.css': 'src/scss/styles.scss',
                }
            },
            dev: {
                options: {
                    style: 'expanded',
                },
                files: {
                    'src/css/styles.css': 'src/scss/styles.scss',
                }
            }
        },
        watch: {
            src: {
                files: ['src/scss/**/*.scss'],
                tasks: ['sass:dist', 'sass:dev']
            }
        }
    });
    grunt.loadNpmTasks('grunt-contrib-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.registerTask('default', ['watch']);
};
